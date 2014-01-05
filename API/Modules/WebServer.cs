using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace TrifleJS.API.Modules
{
    /// <summary>
    /// A HTTP Server class for use inside V8 engine
    /// </summary>
    public class WebServer
    {
        private static Dictionary<string, WebServer> activeBindings = new Dictionary<string, WebServer>();
        private static Dictionary<string, Connection> connections = new Dictionary<string, Connection>();
        private static int concurrentThreads = 0;
        private static int allowedThreads = 2;
        private HttpListener listener;
        private Uri binding;

        public WebServer() {
            listener = new HttpListener();
        }

        /// <summary>
        /// Opens a HTTP listener on specific TCP bindings
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="callbackId"></param>
        public void Listen(string binding, string callbackId) {
            // Start & Run HTTP daemon
            try
            {
                // Initialize URI for binding
                int port;
                if (Int32.TryParse(binding, out port)) { this.binding = new Uri(String.Format("http://localhost:{0}/", port)); }
                else if (!binding.Contains("http")) { this.binding = new Uri(String.Format("http//{0}", binding)); }
                else { this.binding = new Uri(binding); }
                listener.Prefixes.Add(this.binding.AbsoluteUri);
                listener.Start();
                Console.xdebug(String.Format("WebServer:Listening on {0} from thread {1}", binding, global::System.AppDomain.GetCurrentThreadId()));
                activeBindings.Add(callbackId, this);
            }
            catch (Exception ex)
            {
                Console.error(String.Format("Error listening on binding: {0}", binding));
                return;
            }
        }

        /// <summary>
        /// The port where the server is listening
        /// </summary>
        public int Port {
            get { return (this.binding != null) ? this.binding.Port : 0; }
        } 

        /// <summary>
        /// Processes all current requests
        /// </summary>
        internal static void ProcessRequests()
        {
            // Loop through active TCP bindings
            foreach (string callbackId in activeBindings.Keys)
            {
                WebServer server = activeBindings[callbackId];
                if (server != null && server.listener != null && server.listener.IsListening)
                {
                    // Check number of threads listening to incoming connections
                    if (concurrentThreads < allowedThreads)
                    {
                        // Add separate thread for filling up queue
                        concurrentThreads++;
                        server.listener.BeginGetContext(delegate(IAsyncResult result)
                        {
                            try
                            {
                                // Add connection to queue (asynchronously)
                                HttpListenerContext context = server.listener.EndGetContext(result);
                                Connection connection = new Connection(callbackId, context);
                                Console.debug(String.Format("ProcessRequests:Queueing connection for {0}!", connection.id));
                                // This will be processed in STA thread (below)
                                // so that there are no memory conflicts in
                                // callbacks to V8 environment.
                                connections.Add(connection.id, connection);
                            }
                            catch (Exception ex)
                            {
                                Console.error(String.Format("Error queueing connection: {0}", ex.Message));
                            }
                            finally
                            {
                                concurrentThreads--;
                            }
                        }, server.listener);
                    }
                }
                else {
                    activeBindings.Remove(callbackId);
                }
            }
            // Process queue (in STA thread)
            string[] processQueue = new string[connections.Count];
            try
            {
                // Sometimes a new connection gets inserted
                // into the queue asyncronously, causing
                // the statement below to fail.
                // In these cases we just ignore the error
                // and wait for the next pass to read the queue.
                connections.Keys.CopyTo(processQueue, 0);
                // Loop through connections in process queue
                foreach (string connectionId in processQueue)
                {
                    Connection connection = connections[connectionId];
                    if (connection != null && !connection.isProcessing)
                    {
                        // Start processing
                        connection.isProcessing = true;
                        if (connection.request != null && connection.response != null)
                        {
                            // Make callback to V8 environment
                            Console.debug(String.Format("ListenAll:Processing request for {0}!", connectionId));
                            Callback.Execute(connection.callbackId, connectionId);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets the request object for a connection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Request GetRequest(string connectionId) {
            Connection connection = connections[connectionId];
            if (connection != null) {
                return connection.request;
            }
            return null;
        }

        /// <summary>
        /// Gets the response object for a connection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Response GetResponse(string connectionId) {
            Connection connection = connections[connectionId];
            if (connection != null)
            {
                return connection.response;
            }
            return null;
        }

        /// <summary>
        /// The request object used by V8 engine
        /// </summary>
        public class Request { 
        
            public Request(HttpListenerRequest request) {
                this.request = request;
            }

            private HttpListenerRequest request;

            public string Read() {
                string body;
                using (StreamReader stream = new StreamReader(request.InputStream)) {
                    body = stream.ReadToEnd();
                }
                return body;
            }

        }

        /// <summary>
        /// The response object used by V8 engine
        /// </summary>
        public class Response {

            public Response(HttpListenerResponse response, string connectionId) {
                this.response = response;
                this.connectionId = connectionId;
                this.headers = new Dictionary<string, string>();
                this.response.SendChunked = true;
                this.isHeaderSent = false;
            }

            private HttpListenerResponse response;
            private string connectionId;
            private bool isHeaderSent = false;

            /// <summary>
            /// HTTP Status code to send to client
            /// </summary>
            public int statusCode
            {
                get { return response.StatusCode; }
                set { response.StatusCode = value; }
            }

            /// <summary>
            /// Write the response to outgoing stream.
            /// </summary>
            /// <param name="text"></param>
            public void write(string text) {
                // First request requires syncing headers
                if (!this.isHeaderSent) {
                    response.Headers.Clear();
                    foreach (string key in headers.Keys)
                    {
                        response.AddHeader(key, headers[key]);
                    }
                }
                // Write to outgoing stream
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Flush();
                this.isHeaderSent = true;
            }

            /// <summary>
            /// Sends header information to the browser
            /// </summary>
            /// <param name="statusCode"></param>
            /// <param name="headers"></param>
            public void writeHead(int statusCode, Dictionary<string, string> headers) {
                // Set status code and headers
                this.response.StatusCode = statusCode;
                // Use default headers if none found
                if (headers == null) { headers = this.headers; }
                else { this.headers = headers; }
                // Send a newline response.
                // Assuming request.SendChunked = true,
                // this will send off the header information
                this.write(Environment.NewLine);
            }

            /// <summary>
            /// Sets a header
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public void setHeader(string name, string value) {
                response.Headers.Add(name, value);
                headers.Add(name, value);
            }

            /// <summary>
            /// Returns a header
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string header(string name) {
                return headers[name];
            }

            /// <summary>
            /// Collection of response headers to send to browser
            /// </summary>
            public Dictionary<string, string> headers;

            /// <summary>
            /// Sets encoding for HTTP response
            /// </summary>
            /// <param name="encoding"></param>
            public void setEncoding(string encoding) {
                try
                {
                    Encoding enc = Encoding.GetEncoding(encoding);
                    response.ContentEncoding = enc;
                }
                catch { }
            }

            /// <summary>
            /// Writes to buffer and closes connection
            /// </summary>
            public void close() {
                response.Close();
                connections.Remove(this.connectionId);
            }

            public void closeGracefully() { 
                
            }
        }

        /// <summary>
        /// A connection object used for queueing 
        /// incoming connections asynchronously
        /// and processing them on STA thread
        /// </summary>
        public class Connection {

            public Connection(string callbackId, HttpListenerContext context) {
                this.callbackId = callbackId;
                this.id = Guid.NewGuid().ToString().Substring(0, 8);
                this.isProcessing = false;
                this.request = new Request(context.Request);
                this.response = new Response(context.Response, this.id);
            }

            public string id;
            public bool isProcessing;
            public string callbackId;
            public Request request;
            public Response response;

        }
    }
}
