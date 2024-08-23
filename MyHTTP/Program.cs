using System.Net;
using System.Text;

namespace MyHTTP
{
    internal class Program
    {
        static Thread threawdListener;
        static void Main(string[] args)
        {
            threawdListener = new Thread(new ParameterizedThreadStart(Start));
            threawdListener.Start("http://localhost:12345/");
        }

        //Площадь прямоугольного параллелепипида
        public static void Start(object prefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix.ToString());
            listener.Start();
            Console.WriteLine("HTTP сервер запущен и прослушивает...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string responseString;

                if (request.HttpMethod == "POST")
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string postData = reader.ReadToEnd();
                        string[] values = postData.Split('&');

                        try
                        {
                            double a = double.Parse(values[0].Split('=')[1]);
                            double b = double.Parse(values[1].Split('=')[1]);
                            double c = double.Parse(values[2].Split('=')[1]);
                            var s = SurfaceArea(a, b, c);
                            if (s == 0)
                            {
                                responseString = $"<HTML><BODY> No Correct </BODY></HTML>";
                            }
                            else
                            {
                                responseString = $"<HTML><BODY> S =  {SurfaceArea(a, b, c)}</BODY></HTML>";
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            responseString = $"<HTML><BODY>Ошибка: {ex.Message}.</BODY></HTML>";
                        }
                    }
                }
                else
                {
                    responseString = "<HTML><BODY>Метод не поддерживается. Пожалуйста, используйте POST-запрос.</BODY></HTML>";
                }

                HttpListenerResponse response = context.Response;
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static double SurfaceArea(double a, double b, double c)
        {
            var res = 2 * (a * b + b * c + a * c);
            if(res > 0)
                return res;
            else
                return 0;
        }

    }
}