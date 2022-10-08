using System;
using NUnit.Framework;
using SimpleScanner;
using SimpleParser;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestASTParser
{
    public class ASTParserTests
    {
        public static JObject Parse(string text)
        {
            Scanner scanner = new Scanner();
            scanner.SetSource(text, 0);

            Parser parser = new Parser(scanner);

            if (!parser.Parse())
            {
                Assert.Fail("программа не распознана");
            }
            else
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.Formatting = Formatting.Indented;
                jsonSettings.TypeNameHandling = TypeNameHandling.All;
                string output = JsonConvert.SerializeObject(parser.root, jsonSettings);
                return JObject.Parse(output);
            }

            return null;
        }
    }
    
    [TestFixture]
    public class WhileTests
    {
        [Test]
        public void TestWhile()
        {
            var tree = ASTParserTests.Parse("begin while 2 do a:=2 end");
            Assert.AreEqual("ProgramTree.WhileNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);   
            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat"]["$type"]);
        }
    }
    
    [TestFixture]
    public class RepeatTests
    {
        [Test]
        public void TestRepeat()
        {
            var tree = ASTParserTests.Parse("begin repeat a:=2 until 2 end");
            Assert.AreEqual("ProgramTree.RepeatNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());
            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
        }
    }
    
    [TestFixture]
    public class ForTests
    {
        [Test]
        public void TestFor()
        {
            var tree = ASTParserTests.Parse("begin for i:=2 to 10 do a:=2 end");
            Assert.AreEqual("ProgramTree.ForNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);

            Assert.AreEqual("ProgramTree.IdNode, SimpleLang", (string)tree["StList"]["$values"][0]["Id"]["$type"]);
            Assert.AreEqual("i", (string)tree["StList"]["$values"][0]["Id"]["Name"]);

            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr1"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr1"]["Num"]).Trim());

            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr2"]["$type"]);
            Assert.AreEqual("10", ((string)tree["StList"]["$values"][0]["Expr2"]["Num"]).Trim());

            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat"]["$type"]);
        }
    }
    
    [TestFixture]
    public class WriteTests
    {   
        [Test]
        public void TestWrite()
        {
            var tree = ASTParserTests.Parse("begin write(2) end");
            Assert.AreEqual("ProgramTree.WriteNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);

            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());
        }
    }
    
    [TestFixture]
    public class ExtraTests
    {
        [Test]
        public void TestIf()
        {
            // Short if
            var tree = ASTParserTests.Parse("begin if 1 then write(1) end");
            Assert.AreEqual("ProgramTree.IfNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);

            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
            Assert.AreEqual("1", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());

            Assert.AreEqual("ProgramTree.WriteNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat1"]["$type"]);
            Assert.AreEqual("1", (string)tree["StList"]["$values"][0]["Stat1"]["Expr"]["Num"]);

            // Complete if
            tree = ASTParserTests.Parse("begin if 2 then write(2) else write(3) end");
            Assert.AreEqual("ProgramTree.IfNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);

            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());

            Assert.AreEqual("ProgramTree.WriteNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat1"]["$type"]);
            Assert.AreEqual("2", (string)tree["StList"]["$values"][0]["Stat1"]["Expr"]["Num"]);

            Assert.AreEqual("ProgramTree.WriteNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat2"]["$type"]);
            Assert.AreEqual("3", (string)tree["StList"]["$values"][0]["Stat2"]["Expr"]["Num"]);
        }
        
        [Test]
        public void TestVarDef()
        {
            Assert.Fail();
            // TODO: дописать тест
        }
        
        [Test]
        public void TestBinary()
        {
            Assert.Fail();
            // TODO: дописать тест
        }
    }
}