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
        private static Dictionary<string, HttpListener> activeBindings = new Dictionary<string, HttpListener>();
        private static Dictionary<string, Connection> connections = new Dictionary<string, Connection>();
        private static int concurrentThreads = 0;
        private static int allowedThreads = 3;
        private HttpListener listener;
        private Uri binding;

        /// <summary>
        /// Opens a HTTP listener on specific TCP bindings
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="callbackId"></param>
        public bool Listen(string binding, string callbackId) {
            // Start & Run HTTP daemon
            try
            {
                // Initialize URI for binding
                int port;
                if (Int32.TryParse(binding, out port)) { this.binding = new Uri(String.Format("http://localhost:{0}/", port)); }
                else if (!binding.Contains("http")) { this.binding = new Uri(String.Format("http//{0}", binding)); }
                else { this.binding = new Uri(binding); }
                listener = new HttpListener();
                listener.Prefixes.Add(this.binding.AbsoluteUri);
                listener.Start();
                Console.xdebug(String.Format("WebServer:Listening on {0}", this.binding.AbsoluteUri));
                activeBindings.Add(callbackId, listener);
                return true;
            }
            catch (Exception ex)
            {
                Console.error(String.Format("Error listening on binding: {0}", binding));
                this.listener = null;
                this.binding = null;
                return false;
            }
        }

        /// <summary>
        /// The port where the server is listening
        /// </summary>
        public string Port {
            get { return (this.binding != null) ? this.binding.Port.ToString() : ""; }
        }

        /// <summary>
        /// Shuts down the server
        /// </summary>
        public void Close() {
            if (this.listener != null) {
                this.listener.Stop();
                this.listener = null;
                connections.Clear();
            }
        }

        /// <summary>
        /// Processes all current requests
        /// </summary>
        internal static void ProcessConnections()
        {
            // Loop through active TCP bindings
            foreach (string callbackId in activeBindings.Keys)
            {
                // Check each binding for incoming connections
                HttpListener listener = activeBindings[callbackId];
                if (listener != null && listener.IsListening)
                {
                    // Are there enough threads listening to incoming connections?
                    if (concurrentThreads < allowedThreads)
                    {
                        // Add separate thread for filling up queue
                        concurrentThreads++;
                        listener.BeginGetContext(delegate(IAsyncResult result)
                        {
                            try
                            {
                                // Add connection to queue (asynchronously)
                                HttpListenerContext context = listener.EndGetContext(result);
                                Connection connection = new Connection(callbackId, context);
                                Console.xdebug(String.Format("ProcessRequests:Queueing connection for {0}!", connection.id));
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
                        }, listener);
                    }
                }
            }

            // Process queue (in STA thread) to avoid COM memory issues
            try
            {
                // Sometimes a new connection gets inserted
                // into the queue asyncronously, causing
                // the new List<string>() statement below to fail.
                // In these cases we just ignore the error
                // and wait for the next pass to read the queue.
                foreach (string connectionId in new List<string>(connections.Keys))
                {
                    Connection connection = connections[connectionId];
                    if (connection != null && !connection.isProcessing)
                    {
                        // Start processing
                        connection.isProcessing = true;
                        if (connection.request != null && connection.response != null)
                        {
                            // Make callback to V8 environment
                            Console.xdebug(String.Format("ListenAll:Processing request for {0}!", connectionId));
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
                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    this.rawPost = reader.ReadToEnd();
                    if (request.ContentType != null && request.ContentType.Contains("application/x-www-form-urlencoded")) {
                        Dictionary<string, object> post = new Dictionary<string, object>();
                        string[] fields = this.rawPost.Split('&');
                        Array.Sort<string>(fields);
                        foreach(string field in fields) {
                            string[] fieldData = field.Split('=');
                            if (fieldData.Length > 1) {
                                post.Add(fieldData[0], fieldData[1]);
                            }
                        }
                        this.post = post;
                    } else {
                        this.post = rawPost;
                    }
                }
            }

            private HttpListenerRequest request;

            /// <summary>
            /// Defines the request method ('GET', 'POST', etc.)
            /// </summary>
            public string method {
                get { return request.HttpMethod; }
            }

            /// <summary>
            /// The path part and query string part (if any) of the request URL
            /// </summary>
            public string url {
                get { return request.Url.PathAndQuery; }
            }

            /// <summary>
            /// The actual HTTP version
            /// </summary>
            public string httpVersion {
                get { return request.ProtocolVersion.ToString(); }
            }

            /// <summary>
            /// All of the HTTP headers as key-value pairs
            /// </summary>
            public Dictionary<string, object> headers {
                get {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    foreach (string key in request.Headers.AllKeys) {
                        result.Add(key, request.Headers[key]);
                    }
                    return result;
                }
            }

            /// <summary>
            /// The request body (only for 'POST' and 'PUT' method requests)
            /// </summary>
            public object post { get; set; }

            /// <summary>
            /// If the Content-Type header is set to 'application/x-www-form-urlencoded' 
            /// (the default for form submissions), the original contents of post will 
            /// be stored in this extra property (postRaw) and then post will be 
            /// automatically updated with a URL-decoded version of the data.
            /// </summary>
            public string rawPost { get; set; }

        }

        /// <summary>
        /// The response object used by V8 engine
        /// </summary>
        public class Response {

            public Response(HttpListenerResponse response, string connectionId) {
                this.response = response;
                this.connectionId = connectionId;
                this.headers = new Dictionary<string, object>();
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
                        response.AddHeader(key, headers[key].ToString());
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
            public void writeHead(int statusCode, Dictionary<string, object> headers) {
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
                headers.Add(name, value);
            }

            /// <summary>
            /// Returns a header
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string header(string name) {
                return headers[name].ToString();
            }

            /// <summary>
            /// Collection of response headers to send to browser
            /// </summary>
            public Dictionary<string, object> headers { get; set; }

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
            /// Closes connection
            /// </summary>
            public void close() {
                response.Close();
                connections.Remove(this.connectionId);
            }

            /// <summary>
            /// Ensures header information is sent and closes connection
            /// </summary>
            public void closeGracefully() {
                if (!this.isHeaderSent) {
                    this.response.StatusCode = 200;
                    this.write(Environment.NewLine);
                }
                this.close();
            }
        }

        /// <summary>
        /// A connection object used for queueing incoming connections
        /// asynchronously & processing them on STA thread.
        /// </summary>
        public class Connection {

            public Connection(string callbackId, HttpListenerContext context) {
                this.callbackId = callbackId;
                this.id = Utils.newUid();
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
