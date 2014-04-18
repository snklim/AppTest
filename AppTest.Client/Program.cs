﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AppTest.MainLogic;

namespace AppTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            string type = null;

            Regex regex = new Regex(@"\s*type\s*=\s*(\w+)\s*");

            foreach (string a in args)
            {
                if (regex.IsMatch(a))
                {
                    type = regex.Match(a).Groups[1].Value;
                }
            }

            if (string.IsNullOrEmpty(type))
            {
                return;
            }

            HttpChannel channel = null;
            int port = 9001;

            while (true)
            {
                try
                {
                    channel = new HttpChannel(port);
                    ChannelServices.RegisterChannel(channel, false);
                    break;
                }
                catch (SocketException)
                {
                    port++;
                    continue;
                }
            }

            string serverHostUrl = ConfigurationManager.AppSettings["ServerHostUrl"];

            int attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    object obj = Activator.GetObject(typeof(FileProcessor), serverHostUrl);
                    FileProcessor fileProcessor = (FileProcessor)obj;

                    fileProcessor.Subscribe(type, new ClientChannel());

                    break;
                }
                catch (WebException ex)
                {
                    Console.WriteLine("{0}, {1} attempt. Will try again after 1 second", ex.Message, attempt);
                }
                Thread.Sleep(1000);
            }

            Console.WriteLine("Client listening callbacks at {0} port for type: {1}", port, type);

            Console.ReadLine();
        }
    }
}
