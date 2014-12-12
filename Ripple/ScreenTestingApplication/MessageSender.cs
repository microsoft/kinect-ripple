using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTestingApplication
{
    public static class MessageSender
    {
        public static void SendMessage(String optionVal)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "RipplePipe", PipeDirection.Out, PipeOptions.Asynchronous))
            {
                try
                {
                    pipeClient.Connect(2000);
                }
                catch (Exception)
                {
                    //Try once more
                    try
                    {
                        pipeClient.Connect(5000);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                //Connected to the server or floor application
                using (StreamWriter sw = new StreamWriter(pipeClient))
                {
                    sw.Write(optionVal);
                }
            }
        }
    }
}
