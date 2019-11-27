using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using ObjectDumping.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDumping.Tests
{
    [Collection(TestCollections.CultureSpecific)]
    public class ObjectDumperHtmlTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperHtmlTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void OuterElementIsDiv()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            // Act
            var html = ObjectDumperHtml.Dump(person);
            var htmlDoc = html.ToHtmlDocument();
            // Assert
            this.testOutputHelper.WriteLine(html.Escape());
            htmlDoc.DocumentNode.FirstChild.Name.Should().Be("div");
        }

        [Fact]
        public void OuterElementIsDivHasClasses()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            // Act
            var html = ObjectDumperHtml.Dump(person);
            var htmlDoc = html.ToHtmlDocument();
            var outerElement = htmlDoc.DocumentNode.FirstChild;
            // Assert
            this.testOutputHelper.WriteLine(html.Escape());
            outerElement.HasClass("obj-dump").Should().BeTrue();
            outerElement.HasClass("obj-type-Person").Should().BeTrue();
        }

        [Fact]
        public void OuterElementHasTypeInfo()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            // Act
            var html = ObjectDumperHtml.Dump(person);
            var htmlDoc = html.ToHtmlDocument();
            var outerElement = htmlDoc.DocumentNode.FirstChild;
            var typeInfoElement = outerElement.FirstChild;
            // Assert
            this.testOutputHelper.WriteLine(typeInfoElement.OuterHtml);
            typeInfoElement.Name.Should().Be("div");
            typeInfoElement.HasClass("obj-type-info").Should().BeTrue();
            typeInfoElement.InnerText.Should().Be("Dumped Type: Person");

            var span1 = typeInfoElement.FirstChild;
            var span2 = typeInfoElement.LastChild;
            span1.Name.Should().Be("span");
            span1.HasClass("obj-label").Should().BeTrue();            

            span2.Name.Should().Be("span");
            span2.HasClass("obj-type").Should().BeTrue();
        }



        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();

            // Act
            var dump = ObjectDumperHtml.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var listPerson = new List<Person>\r\n{\r\n  new Person\r\n  {\r\n    Name = \"Person 1\",\r\n    Char = '\\0',\r\n    Age = 3,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  },\r\n  new Person\r\n  {\r\n    Name = \"Person 2\",\r\n    Char = '\\0',\r\n    Age = 3,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpNestedObjects()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };

            // Act
            var dump = ObjectDumperHtml.Dump(organization);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var organization = new Organization\r\n{\r\n  Name = \"superdev gmbh\",\r\n  Persons = new List<Person>\r\n  {\r\n    new Person\r\n    {\r\n      Name = \"Person 1\",\r\n      Char = '\\0',\r\n      Age = 3,\r\n      GetOnly = 11,\r\n      Bool = false,\r\n      Byte = 0,\r\n      ByteArray = new Byte[]\r\n      {\r\n        1,\r\n        2,\r\n        3,\r\n        4\r\n      },\r\n      SByte = 0,\r\n      Float = 0f,\r\n      Uint = 0,\r\n      Long = 0L,\r\n      ULong = 0L,\r\n      Short = 0,\r\n      UShort = 0,\r\n      Decimal = 0m,\r\n      Double = 0d,\r\n      DateTime = DateTime.MinValue,\r\n      NullableDateTime = null,\r\n      Enum = System.DateTimeKind.Unspecified\r\n    },\r\n    new Person\r\n    {\r\n      Name = \"Person 2\",\r\n      Char = '\\0',\r\n      Age = 3,\r\n      GetOnly = 11,\r\n      Bool = false,\r\n      Byte = 0,\r\n      ByteArray = new Byte[]\r\n      {\r\n        1,\r\n        2,\r\n        3,\r\n        4\r\n      },\r\n      SByte = 0,\r\n      Float = 0f,\r\n      Uint = 0,\r\n      Long = 0L,\r\n      ULong = 0L,\r\n      Short = 0,\r\n      UShort = 0,\r\n      Decimal = 0m,\r\n      Double = 0d,\r\n      DateTime = DateTime.MinValue,\r\n      NullableDateTime = null,\r\n      Enum = System.DateTimeKind.Unspecified\r\n    }\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpMultipleGenericTypes()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var genericClass = new GenericClass<string, float, Person> { Prop1 = "Test", Prop2 = 123.45f, Prop3 = person };

            // Act
            var dump = ObjectDumperHtml.Dump(genericClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var genericClass = new GenericClass<String, Single, Person>\r\n{\r\n  Prop1 = \"Test\",\r\n  Prop2 = 123.45f,\r\n  Prop3 = new Person\r\n  {\r\n    Name = \"Person 1\",\r\n    Char = '\\0',\r\n    Age = 2,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpObjectWithMaxLevel()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };
            var options = new DumpOptions { MaxLevel = 1 };

            // Act
            var dump = ObjectDumperHtml.Dump(organization, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("var organization = new Organization\r\n{\r\n  Name = \"superdev gmbh\",\r\n  Persons = new List<Person>\r\n  {\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldExcludeProperties()
        {
            // Arrange
            var testObject = new TestObject();
            var options = new DumpOptions { ExcludeProperties = { "Id", "NonExistent" } };

            // Act
            var dump = ObjectDumperHtml.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("var testObject = new TestObject\r\n{\r\n  NullableDateTime = null\r\n};");
        }

        [Fact]
        public void ShouldOrderProperties()
        {
            // Arrange
            var testObject = new OrderPropertyTestObject();
            var options = new DumpOptions { PropertyOrderBy = p => p.Name };

            // Act
            var dump = ObjectDumperHtml.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("var orderPropertyTestObject = new OrderPropertyTestObject\r\n{\r\n  A = null,\r\n  B = null,\r\n  C = null\r\n};");
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Unspecified()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Unspecified);

            // Act
            var dump = ObjectDumperHtml.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be("<div class=\"obj-dump obj-type-DateTime\"><div class=\"obj-type-info\"><span class=\"obj-label\">Dumped Type:</span> <span class=\"obj-type\">DateTime</span></div><span class=\"obj-value obj-type-DateTime\">2000-01-01T23:59:59.0000000</span></div>");
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Utc()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var dump = ObjectDumperHtml.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be("<div class=\"obj-dump obj-type-DateTime\"><div class=\"obj-type-info\"><span class=\"obj-label\">Dumped Type:</span> <span class=\"obj-type\">DateTime</span></div><span class=\"obj-value obj-type-DateTime\">2000-01-01T23:59:59.0000000Z</span></div>");
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Local()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Local);

            // Act
            var dump = ObjectDumperHtml.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be($"<div class=\"obj-dump obj-type-DateTime\"><div class=\"obj-type-info\"><span class=\"obj-label\">Dumped Type:</span> <span class=\"obj-type\">DateTime</span></div><span class=\"obj-value obj-type-DateTime\">2000-01-01T23:59:59.0000000-05:00</span></div>");
        }

        [Fact]
        public void ShouldDumpDateTimeMinValue()
        {
            // Arrange
            var datetime = DateTime.MinValue;

            // Act
            var dump = ObjectDumperHtml.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be("<div class=\"obj-dump obj-type-DateTime\"><div class=\"obj-type-info\"><span class=\"obj-label\">Dumped Type:</span> <span class=\"obj-type\">DateTime</span></div><span class=\"obj-value obj-type-DateTime\">0001-01-01T00:00:00.0000000</span></div>");
        }

        [Fact]
        public void ShouldDumpDateTimeMaxValue()
        {
            // Arrange
            var datetime = DateTime.MaxValue;

            // Act
            var dump = ObjectDumperHtml.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be("<div class=\"obj-dump obj-type-DateTime\"><div class=\"obj-type-info\"><span class=\"obj-label\">Dumped Type:</span> <span class=\"obj-type\">DateTime</span></div><span class=\"obj-value obj-type-DateTime\">9999-12-31T23:59:59.9999999</span></div>");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            DateTime? datetime = null;

            // Act
            var dump = ObjectDumperHtml.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump.Escape());
            dump.Should().NotBeNull();
            dump.Should().Be("var x = null;");
        }

        [Fact]
        public void ShouldDumpEnum()
        {
            // Arrange
            var dateTimeKind = DateTimeKind.Utc;

            // Act
            var dump = ObjectDumperHtml.Dump(dateTimeKind);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTimeKind = System.DateTimeKind.Utc;");
        }

        [Fact]
        public void ShouldDumpGuid()
        {
            // Arrange
            var guid = new Guid("024CC229-DEA0-4D7A-9FC8-722E3A0C69A3");

            // Act
            var dump = ObjectDumperHtml.Dump(guid);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var guid = new Guid(\"024cc229-dea0-4d7a-9fc8-722e3a0c69a3\");");
        }

        [Fact]
        public void ShouldDumpDictionary()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>
            {
                { 1, "Value1" },
                { 2, "Value2" },
                { 3, "Value3" }
            };

            // Act
            var dump = ObjectDumperHtml.Dump(dictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dictionaryInt32String = new Dictionary<Int32, String>\r\n{\r\n  { 1, \"Value1\" },\r\n  { 2, \"Value2\" },\r\n  { 3, \"Value3\" }\r\n};");
        }
    }
}
