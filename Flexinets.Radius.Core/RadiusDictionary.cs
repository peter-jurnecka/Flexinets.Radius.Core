﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flexinets.Radius.Core
{
    public class RadiusDictionary : IRadiusDictionary
    {
        internal Dictionary<byte, DictionaryAttribute> Attributes { get; set; } = new Dictionary<byte, DictionaryAttribute>();
        internal List<DictionaryVendorAttribute> VendorSpecificAttributes { get; set; } = new List<DictionaryVendorAttribute>();
        internal Dictionary<string, DictionaryAttribute> AttributeNames { get; set; } = new Dictionary<string, DictionaryAttribute>();
        private readonly ILogger _logger;


        /// <summary>
        /// Load the dictionary from a dictionary file
        /// </summary>        
        public RadiusDictionary(string dictionaryFilePath, ILogger<RadiusDictionary> logger)
        {
            this._logger = logger!;

            using (var sr = new StreamReader(dictionaryFilePath))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (line?.StartsWith("Attribute") == true)
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var key = Convert.ToByte(lineparts[1]);

                        // If duplicates are encountered, the last one will prevail                        
                        if (Attributes.ContainsKey(key))
                        {
                            Attributes.Remove(key);
                        }
                        if (AttributeNames.ContainsKey(lineparts[2]))
                        {
                            AttributeNames.Remove(lineparts[2]);
                        }
                        var attributeDefinition = new DictionaryAttribute(lineparts[2], key, lineparts[3]);
                        Attributes.Add(key, attributeDefinition);
                        AttributeNames.Add(attributeDefinition.Name, attributeDefinition);
                    }

                    if (line?.StartsWith("VendorSpecificAttribute") == true)
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var vsa = new DictionaryVendorAttribute(
                            Convert.ToUInt32(lineparts[1]),
                            lineparts[3],
                            Convert.ToUInt32(lineparts[2]),
                            lineparts[4]);

                        VendorSpecificAttributes.Add(vsa);

                        if (AttributeNames.ContainsKey(vsa.Name))
                        {
                            AttributeNames.Remove(vsa.Name);
                        }
                        AttributeNames.Add(vsa.Name, vsa);
                    }
                }

                _logger?.LogInformation($"Parsed {Attributes.Count} attributes and {VendorSpecificAttributes.Count} vendor attributes from file");
            }
        }


        public DictionaryVendorAttribute? GetVendorAttribute(uint vendorId, byte vendorCode)
        {
            return VendorSpecificAttributes.FirstOrDefault(o => o.VendorId == vendorId && o.VendorCode == vendorCode);
        }

        public DictionaryAttribute GetAttribute(byte typecode)
        {
            return Attributes[typecode];
        }

        public DictionaryAttribute? GetAttribute(string name)
        {
            AttributeNames.TryGetValue(name, out var attributeType);
            return attributeType;
        }
    }
}
