using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Xml;

namespace UnitTests
{
    /// <summary>
    /// Helper utilities
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Loads all announcement messages from file into list.
        /// </summary>
        /// <param name="list">Reference to a list</param>
        /// <param name="path">Path to xml file</param>
        public static void LoadMessages(List<Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>> list, string path)
        {
            MethodInfo loadEndpoint = typeof(EndpointDiscoveryMetadata).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((x) => "ReadFrom" == x.Name);
            DiscoveryMessageSequenceGenerator gen = new DiscoveryMessageSequenceGenerator();

            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing("Envelope", "http://www.w3.org/2003/05/soap-envelope"))
                {
                    using (Message msg = Message.CreateMessage(reader, Int16.MaxValue, MessageVersion.Soap12))
                    {
                        EndpointDiscoveryMetadata data = new EndpointDiscoveryMetadata();

                        loadEndpoint.Invoke(data, new object[] { DiscoveryVersion.WSDiscovery11, msg.GetReaderAtBodyContents() });

                        list.Add(new Tuple<DiscoveryMessageSequence, EndpointDiscoveryMetadata>(gen.Next(), data));
                    }
                }
            }
        }

        /// <summary>
        /// Loads all Probe messages from file into list.
        /// </summary>
        /// <param name="list">Reference to a list</param>
        /// <param name="path">Path to xml file</param>
        public static void LoadMessages(List<FindRequestContext> list, string path)
        {
            MethodInfo loadCriteria = typeof(FindCriteria).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((x) => "ReadFrom" == x.Name);
            ConstructorInfo ctorFindRequestContext = typeof(FindRequestContext).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.ReadToFollowing("Envelope", "http://www.w3.org/2003/05/soap-envelope"))
                {
                    using (Message msg = Message.CreateMessage(reader, Int16.MaxValue, MessageVersion.Soap12))
                    {
                        FindCriteria data = FindCriteria.CreateMetadataExchangeEndpointCriteria();

                        loadCriteria.Invoke(data, new object[] { DiscoveryVersion.WSDiscovery11, msg.GetReaderAtBodyContents() });

                        list.Add((FindRequestContext)ctorFindRequestContext.Invoke(new object[] { data }));
                    }
                }
            }
        }

        /// <summary>
        /// Loads all Probe messages from file into list.
        /// </summary>
        /// <param name="list">Reference to a list</param>
        /// <param name="path">Path to xml file</param>
        public static void LoadMessages(List<ResolveCriteria> list, string path)
        {
            MethodInfo loadCriteria = typeof(FindCriteria).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((x) => "ReadFrom" == x.Name);
            ConstructorInfo ctorFindRequestContext = typeof(FindRequestContext).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();

            //using (XmlReader reader = XmlReader.Create(path))
            //{
            //    while (reader.ReadToFollowing("Envelope", "http://www.w3.org/2003/05/soap-envelope"))
            //    {
            //        using (Message msg = Message.CreateMessage(reader, Int16.MaxValue, MessageVersion.Soap12))
            //        {
            //            FindCriteria data = FindCriteria.CreateMetadataExchangeEndpointCriteria();

            //            loadCriteria.Invoke(data, new object[] { DiscoveryVersion.WSDiscovery11, msg.GetReaderAtBodyContents() });

            //            list.Add((FindRequestContext)ctorFindRequestContext.Invoke(new object[] { data }));
            //        }
            //    }
            //}
        }
    }
}
