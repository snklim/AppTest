using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AppTest.MainLogic;

namespace AppTest.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //new FileProcessor();

            // Set the TypeFilterLevel to Full since
            // callbacks require additional security 
            // requirements
            SoapServerFormatterSinkProvider serverFormatter =
                       new SoapServerFormatterSinkProvider();
            serverFormatter.TypeFilterLevel =
              System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            // we have to change the name since we can't
            // have two channels with the same name.
            Hashtable ht = new Hashtable();
            ht["name"] = "ServerChannel";
            ht["port"] = 9000;

            // now create and register our custom HttpChannel 
            HttpChannel channel = new HttpChannel(ht, null, serverFormatter);
            ChannelServices.RegisterChannel(channel, false);

            // register a WKO type in Singleton mode
            string identifier = "FileProcessor";
            WellKnownObjectMode mode = WellKnownObjectMode.Singleton;

            WellKnownServiceTypeEntry entry =
                new WellKnownServiceTypeEntry(typeof(FileProcessor),
                identifier, mode);
            RemotingConfiguration.RegisterWellKnownServiceType(entry);

            Console.WriteLine("Server started");

            Console.ReadLine();
        }
    }
}
