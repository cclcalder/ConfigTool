using System;
using System.Linq;
using System.Xml.Linq;
using Model.Entity.ROBs;
using Xunit;

namespace Model.Test
{
    public class RobTest
    {
        private const string GetRobsXml = @"<ROB>
							<ID>7</ID>
							<Name>(JD-1090) My ROB</Name>
							<ItemType>Promotion</ItemType>
							<Customer><Name>CRTG</Name><Color>#FFFFFF</Color></Customer>
							<Status><Name>Draft</Name><Color>#ffffff</Color></Status>
							<Attributes>
							  <Attribute><Name>End Date</Name><Value>2012-10-10</Value><Format>dd/MM/yyyy</Format></Attribute>
							  <Attribute><Name>Start Date</Name><Value>2012-09-07</Value><Format>dd/MM/yyyy</Format></Attribute>
							</Attributes>
						  </ROB>";

        [Fact]
        public void RobFromXmlParsesCorrectly()
        {
            var actual = Rob.FromGetRobsXml(XElement.Parse(GetRobsXml));
            Assert.NotNull(actual);
            Assert.Equal("7", actual.ID);
            Assert.Equal("(JD-1090) My ROB", actual.Name);
            Assert.Equal("Promotion", actual.ItemType);
            Assert.Equal("Draft", actual.Name);
            Assert.NotNull(actual.Attributes);
            Assert.Equal(2, actual.Attributes.Count);
            var endDate = actual.Attributes.FirstOrDefault(a => a.Name.Equals("End Date"));
            Assert.NotNull(endDate);
            Assert.Equal(DateTime.Parse("2012-10-10"), endDate.Value);
            Assert.Equal("dd/MM/yyyy", endDate.Format);
            var startDate = actual.Attributes.FirstOrDefault(a => a.Name.Equals("Start Date"));
            Assert.NotNull(startDate);
            Assert.Equal(DateTime.Parse("2012-09-07"), startDate.Value);
            Assert.Equal("dd/MM/yyyy", startDate.Format);
        }

        private const string GetRobXml = @"<Results>
  <ROB>
    <BtnConvertIsEnabled>1</BtnConvertIsEnabled>
    <Name>My ROB</Name>
    <Code>JD-1090</Code>
    <CustLevel>
      <ID>3</ID>
    </CustLevel>
    <ProdLevel>
      <ID>3</ID>
    </ProdLevel>
    <Products>
      <Product>
        <ID>15001</ID>
      </Product>
      <Product>
        <ID>15017</ID>
      </Product>
    </Products>
    <Customers>
      <Customer>
        <ID>20004</ID>
      </Customer>
    </Customers>
    <Type>
      <ID>1</ID>
    </Type>
    <SubType>
      <ID>1</ID>
    </SubType>
    <Dates>
      <Start>2012-09-07</Start>
      <End>2012-10-10</End>
    </Dates>
    <ImpactOptions>
      <Option>
        <ID>1</ID>
        <Value>100.0000000000</Value>
      </Option>
      <Option>
        <ID>4</ID>
        <Value>400.0000000000</Value>
      </Option>
    </ImpactOptions>
    <StatusID>1</StatusID>
    <IsEditable>1</IsEditable>
    <ScenarioID>1</ScenarioID>
  </ROB>
</Results>";
    }
}