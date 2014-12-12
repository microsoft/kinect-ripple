using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RippleScreenApp.Utilities
{
    public static class MessageReceiver
    {
        private static readonly int BufferSize = 256;
        public static string pipeName;
        private static NamedPipeServerStream pipeServer;
        private static ScreenWindow owner;

        public static void StartReceivingMessages(ScreenWindow currentInstance)
        {
            try
            {
                pipeName = "RipplePipe";
                owner = currentInstance;
                ThreadStart pipeThread = new ThreadStart(createPipeServer);
                Thread listenerThread = new Thread(pipeThread);
                listenerThread.SetApartmentState(ApartmentState.STA);
                listenerThread.IsBackground = true;
                listenerThread.Start();
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public static void createPipeServer()
        {
            Decoder decoder = Encoding.Default.GetDecoder();
            Byte[] bytes = new Byte[BufferSize];
            char[] chars = new char[BufferSize];
            int numBytes = 0;
            StringBuilder msg = new StringBuilder();

            try
            {
                pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                while (true)
                {
                    pipeServer.WaitForConnection();

                    do
                    {
                        msg.Length = 0;
                        do
                        {
                            numBytes = pipeServer.Read(bytes, 0, BufferSize);
                            if (numBytes > 0)
                            {
                                int numChars = decoder.GetCharCount(bytes, 0, numBytes);
                                decoder.GetChars(bytes, 0, numBytes, chars, 0, false);
                                msg.Append(chars, 0, numChars);
                            }
                        } while (numBytes > 0 && !pipeServer.IsMessageComplete);
                        decoder.Reset();
                        if (numBytes > 0)
                        {
                            //Notify the UI for message received
                            if (owner != null)
                                owner.Dispatcher.Invoke(DispatcherPriority.Send, new Action<string>(owner.OnMessageReceived), msg.ToString());
                            //ownerInvoker.Invoke(msg.ToString());
                        }
                    } while (numBytes != 0);
                    pipeServer.Disconnect();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SendMessage(String optionVal)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "RippleReversePipe", PipeDirection.Out, PipeOptions.Asynchronous))
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
                    catch (Exception ex)
                    {
                        RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in Send Message {1} at Screen side {0}", ex.Message, optionVal);
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
