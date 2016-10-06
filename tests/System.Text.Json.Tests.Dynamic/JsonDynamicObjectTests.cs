﻿using System.Text.Formatting;
using System.Text.Utf8;
using Xunit;

namespace System.Text.Json.Dynamic.Tests
{
    public class JsonDynamicObjectTests
    {
        [Fact]
        public void NestedEagerReadLazy()
        {
            dynamic json = JsonLazyDynamicObject.Parse(new Utf8String("{ \"FirstName\": \"John\", \"LastName\": \"Smith\", \"Address\": { \"Street\": \"21 2nd Street\", \"City\": \"New York\", \"State\": \"NY\", \"Zip\": \"10021-3100\" }, \"IsAlive\": true, \"Age\": 25, \"Spouse\":null }"));
            Assert.Equal("John", (string)json.FirstName);
            Assert.Equal("Smith", (string)json.LastName);
            Assert.Equal(true, (bool)json.IsAlive);
            Assert.Equal(25, (int)json.Age);
            //Assert.Equal(null, (object)json.Spouse);
            //Assert.Equal(6, (int)json.Count);

            // TODO: enable this
            //dynamic address = json.Address;
            //Assert.Equal(new Utf8String("21 2nd Street"), (Utf8String)address.Street);
            //Assert.Equal(new Utf8String("New York"), (Utf8String)address.City);
            //Assert.Equal(new Utf8String("NY"), (Utf8String)address.State);
            //Assert.Equal(new Utf8String("10021-3100"), (Utf8String)address.Zip);
            //Assert.Equal(4, (int)address.Count);
        }

        [Fact]
        public void NestedEagerRead()
        {
            dynamic json = JsonDynamicObject.Parse(new Utf8String("{ \"FirstName\": \"John\", \"LastName\": \"Smith\", \"Address\": { \"Street\": \"21 2nd Street\", \"City\": \"New York\", \"State\": \"NY\", \"Zip\": \"10021-3100\" }, \"IsAlive\": true, \"Age\": 25, \"Spouse\":null }"));
            Assert.Equal(new Utf8String("John"), json.FirstName);
            Assert.Equal(new Utf8String("Smith"), json.LastName);
            Assert.Equal(true, json.IsAlive);
            Assert.Equal(25, json.Age);
            Assert.Equal(null, json.Spouse);
            Assert.Equal(6, json.Count);

            dynamic address = json.Address;
            Assert.Equal(new Utf8String("21 2nd Street"), address.Street);
            Assert.Equal(new Utf8String("New York"), address.City);
            Assert.Equal(new Utf8String("NY"), address.State);
            Assert.Equal(new Utf8String("10021-3100"), address.Zip);
            Assert.Equal(4, address.Count);
        }

        [Fact]
        public void NestedEagerWrite()
        {
            var jsonText = new Utf8String("{\"FirstName\":\"John\",\"LastName\":\"Smith\",\"Address\":{\"Street\":\"21 2nd Street\",\"City\":\"New York\",\"State\":\"NY\",\"Zip\":\"10021-3100\"},\"IsAlive\":true,\"Age\":25,\"Spouse\":null}");
            JsonDynamicObject json = JsonDynamicObject.Parse(jsonText, 100);
            var formatter = new ArrayFormatter(1024, EncodingData.InvariantUtf8);
            formatter.Append(json);
            var formattedText = new Utf8String(formatter.Formatted);

            // The follwoing check only works given the current implmentation of Dictionary.
            // If the implementation changes, the properties might round trip to different places in the JSON text.
            Assert.Equal(jsonText, formattedText); 
        }

        [Fact]
        public void EagerWrite()
        {
            dynamic json = new JsonDynamicObject();
            json.First = "John";

            var formatter = new ArrayFormatter(1024, EncodingData.InvariantUtf8);
            formatter.Append((JsonDynamicObject)json);
            var formattedText = new Utf8String(formatter.Formatted);
            Assert.Equal(new Utf8String("{\"First\":\"John\"}"), formattedText);
        }

        [Fact]
        public void NonAllocatingRead()
        {
            var jsonText = new Utf8String("{\"First\":\"John\",\"Age\":25}");
            JsonDynamicObject json = JsonDynamicObject.Parse(jsonText);

            Assert.Equal(new Utf8String("John"), json.First());
            Assert.Equal(25U, json.Age());
        }
    }

    static class SchemaExtensions
    {
        static readonly Utf8String s_first = new Utf8String("First");
        static readonly Utf8String s_age = new Utf8String("Age");

        public static Utf8String First(this JsonDynamicObject json)
        {
            Utf8String value;
            if(json.TryGetString(s_first, out value)) {
                return value;
            }
            throw new InvalidOperationException();
        }

        public static uint Age(this JsonDynamicObject json)
        {
            uint value;
            if (json.TryGetUInt32(s_age, out value)) {
                return value;
            }
            throw new InvalidOperationException();
        }
    }
}