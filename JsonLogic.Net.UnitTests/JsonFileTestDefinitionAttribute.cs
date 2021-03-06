using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace JsonLogic.Net.UnitTests
{
    /// <summary>
    /// Based on https://andrewlock.net/creating-a-custom-xunit-theory-test-dataattribute-to-load-data-from-json-files/
    /// Loading data into JSON strings rather than proper JTokens to make the tests look better in the test runner test name
    /// </summary>
    public class JsonFileTestDefinitionAttribute : DataAttribute
    {
        private readonly string _filePath;
        public JsonFileTestDefinitionAttribute(string filePath)
        {
            _filePath = filePath;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            // Load the file
            var fileData = File.ReadAllText(_filePath);

            // load by hand to support existing file structure
            var dataArray = JArray.Parse(fileData);

            return dataArray.Where(item => item is JArray && ((JArray) item).Count >= 3)
                .Select(item => new object[] { item[0].ToString(Formatting.None), item[1].ToString(Formatting.None), item[2].ToString(Formatting.None) });
        }

        
    }
}