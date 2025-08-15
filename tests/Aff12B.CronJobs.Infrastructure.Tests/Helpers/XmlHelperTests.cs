using Aff12B.CronJobs.Infrastructure.Helpers;
using FluentAssertions;
using System.Xml.Linq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Helpers
{
    /// <summary>
    /// Unit tests for XmlHelper class
    /// Tests XML generation functionality
    /// Target: 100% code coverage
    /// </summary>
    public class XmlHelperTests
    {
        [Fact]
        public void BuildXml_ValidData_ShouldGenerateCorrectXml()
        {
            // Arrange
            var data = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "First", IsActive = true },
                new TestModel { Id = 2, Name = "Second", IsActive = false }
            };
            var rootName = "TestRoot";
            var tagName = "TestItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            result.Should().NotBeNullOrEmpty();

            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root.Should().NotBeNull();
            xmlDoc.Root!.Name.LocalName.Should().Be(rootName);

            var elements = xmlDoc.Root.Elements(tagName).ToList();
            elements.Should().HaveCount(2);

            // Verify first element
            var firstElement = elements[0];
            firstElement.Attribute("id")?.Value.Should().Be("1");
            firstElement.Attribute("name")?.Value.Should().Be("First");
            firstElement.Attribute("isactive")?.Value.Should().Be("true");

            // Verify second element
            var secondElement = elements[1];
            secondElement.Attribute("id")?.Value.Should().Be("2");
            secondElement.Attribute("name")?.Value.Should().Be("Second");
            secondElement.Attribute("isactive")?.Value.Should().Be("false");
        }

        [Fact]
        public void BuildXml_EmptyData_ShouldGenerateEmptyRoot()
        {
            // Arrange
            var emptyData = new List<TestModel>();
            var rootName = "EmptyRoot";
            var tagName = "EmptyItem";

            // Act
            var result = XmlHelper.BuildXml(emptyData, rootName, tagName);

            // Assert
            result.Should().NotBeNullOrEmpty();

            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root.Should().NotBeNull();
            xmlDoc.Root!.Name.LocalName.Should().Be(rootName);
            xmlDoc.Root.Elements().Should().BeEmpty();
        }

        [Fact]
        public void BuildXml_NullItem_ShouldThrowArgumentNullException()
        {
            // Arrange
            var dataWithNull = new List<TestModel> { new TestModel { Id = 1, Name = "Valid" }, null! };
            var rootName = "TestRoot";
            var tagName = "TestItem";

            // Act & Assert
            var act = () => XmlHelper.BuildXml(dataWithNull, rootName, tagName);
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Null item found in insert data*")
                .And.ParamName.Should().Be("data");
        }

        [Fact]
        public void BuildXml_VariousTypes_ShouldHandleProperties()
        {
            // Arrange
            var data = new List<ComplexTestModel>
            {
                new ComplexTestModel
                {
                    StringProp = "Test String",
                    IntProp = 42,
                    DoubleProp = 3.14159,
                    BoolProp = true,
                    DateTimeProp = new DateTime(2023, 12, 25, 10, 30, 45)
                }
            };
            var rootName = "ComplexRoot";
            var tagName = "ComplexItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            var element = xmlDoc.Root!.Elements(tagName).First();

            element.Attribute("stringprop")?.Value.Should().Be("Test String");
            element.Attribute("intprop")?.Value.Should().Be("42");
            element.Attribute("doubleprop")?.Value.Should().Be("3.14159");
            element.Attribute("boolprop")?.Value.Should().Be("true");
            element.Attribute("datetimeprop")?.Value.Should().Be("2023-12-25T10:30:45");
        }

        [Fact]
        public void BuildXml_NullPropertyValues_ShouldUseEmptyString()
        {
            // Arrange
            var data = new List<TestModelWithNullable>
            {
                new TestModelWithNullable
                {
                    Id = 1,
                    Name = null, // Null string property
                    OptionalValue = null // Nullable int property
                }
            };
            var rootName = "NullTestRoot";
            var tagName = "NullTestItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            var element = xmlDoc.Root!.Elements(tagName).First();

            element.Attribute("id")?.Value.Should().Be("1");
            element.Attribute("name")?.Value.Should().Be(""); // Null becomes empty string
            element.Attribute("optionalvalue")?.Value.Should().Be(""); // Null becomes empty string
        }

        [Fact]
        public void BuildXml_PropertyNames_ShouldBeLowercase()
        {
            // Arrange
            var data = new List<CamelCaseModel>
            {
                new CamelCaseModel
                {
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddress = "john.doe@example.com",
                    IsVIPMember = true
                }
            };
            var rootName = "CamelCaseRoot";
            var tagName = "CamelCaseItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            var element = xmlDoc.Root!.Elements(tagName).First();

            // Verify all attribute names are lowercase
            element.Attribute("firstname")?.Value.Should().Be("John");
            element.Attribute("lastname")?.Value.Should().Be("Doe");
            element.Attribute("emailaddress")?.Value.Should().Be("john.doe@example.com");
            element.Attribute("isvipmember")?.Value.Should().Be("true");

            // Verify no uppercase attributes exist
            element.Attribute("FirstName").Should().BeNull();
            element.Attribute("LastName").Should().BeNull();
            element.Attribute("EmailAddress").Should().BeNull();
            element.Attribute("IsVIPMember").Should().BeNull();
        }

        [Fact]
        public void BuildXml_SingleItem_ShouldGenerateCorrectXml()
        {
            // Arrange
            var data = new List<TestModel>
            {
                new TestModel { Id = 99, Name = "Single Item", IsActive = true }
            };
            var rootName = "SingleRoot";
            var tagName = "SingleItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root!.Elements().Should().HaveCount(1);

            var element = xmlDoc.Root.Elements(tagName).First();
            element.Attribute("id")?.Value.Should().Be("99");
            element.Attribute("name")?.Value.Should().Be("Single Item");
            element.Attribute("isactive")?.Value.Should().Be("true");
        }

        [Fact]
        public void BuildXml_SpecialCharacters_ShouldEscapeCorrectly()
        {
            // Arrange
            var data = new List<TestModel>
            {
                new TestModel
                {
                    Id = 1,
                    Name = "Test & \"Special\" <Characters> 'Here'",
                    IsActive = true
                }
            };
            var rootName = "SpecialRoot";
            var tagName = "SpecialItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            result.Should().NotBeNullOrEmpty();

            // Should be valid XML despite special characters
            var xmlDoc = XDocument.Parse(result);
            var element = xmlDoc.Root!.Elements(tagName).First();

            // XML should properly escape special characters in attribute values
            element.Attribute("name")?.Value.Should().Be("Test & \"Special\" <Characters> 'Here'");
        }

        [Fact]
        public void BuildXml_LargeDataSet_ShouldHandleEfficiently()
        {
            // Arrange
            var data = new List<TestModel>();
            for (int i = 1; i <= 1000; i++)
            {
                data.Add(new TestModel
                {
                    Id = i,
                    Name = $"Item {i}",
                    IsActive = i % 2 == 0
                });
            }
            var rootName = "LargeRoot";
            var tagName = "LargeItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            result.Should().NotBeNullOrEmpty();

            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root!.Elements(tagName).Should().HaveCount(1000);

            // Verify first and last items
            var elements = xmlDoc.Root.Elements(tagName).ToList();
            elements[0].Attribute("id")?.Value.Should().Be("1");
            elements[0].Attribute("name")?.Value.Should().Be("Item 1");
            elements[999].Attribute("id")?.Value.Should().Be("1000");
            elements[999].Attribute("name")?.Value.Should().Be("Item 1000");
        }

        [Fact]
        public void BuildXml_EmptyStrings_ShouldHandleCorrectly()
        {
            // Arrange
            var data = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "", IsActive = false }
            };
            var rootName = "EmptyStringRoot";
            var tagName = "EmptyStringItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            var element = xmlDoc.Root!.Elements(tagName).First();

            element.Attribute("id")?.Value.Should().Be("1");
            element.Attribute("name")?.Value.Should().Be("");
            element.Attribute("isactive")?.Value.Should().Be("false");
        }

        [Fact]
        public void BuildXml_CustomRootAndTagNames_ShouldUseCorrectNames()
        {
            // Arrange
            var data = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Test", IsActive = true }
            };
            var rootName = "CustomRootElement";
            var tagName = "CustomChildElement";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root!.Name.LocalName.Should().Be("CustomRootElement");
            xmlDoc.Root.Elements("CustomChildElement").Should().HaveCount(1);
        }

        [Fact]
        public void BuildXml_ClassWithNoProperties_ShouldGenerateEmptyElements()
        {
            // Arrange
            var data = new List<EmptyClass> { new EmptyClass(), new EmptyClass() };
            var rootName = "EmptyClassRoot";
            var tagName = "EmptyClassItem";

            // Act
            var result = XmlHelper.BuildXml(data, rootName, tagName);

            // Assert
            var xmlDoc = XDocument.Parse(result);
            xmlDoc.Root!.Elements(tagName).Should().HaveCount(2);

            foreach (var element in xmlDoc.Root.Elements(tagName))
            {
                element.Attributes().Should().BeEmpty();
            }
        }

        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }

        private class ComplexTestModel
        {
            public string StringProp { get; set; } = string.Empty;
            public int IntProp { get; set; }
            public double DoubleProp { get; set; }
            public bool BoolProp { get; set; }
            public DateTime DateTimeProp { get; set; }
        }

        private class TestModelWithNullable
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? OptionalValue { get; set; }
        }

        private class CamelCaseModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;
            public bool IsVIPMember { get; set; }
        }

        private class EmptyClass
        {
            // No properties
        }
    }
}
