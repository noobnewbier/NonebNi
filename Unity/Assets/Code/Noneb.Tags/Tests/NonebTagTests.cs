using System.Linq;
using Noneb.Tags.Runtime;
using NUnit.Framework;

namespace Noneb.Tags.Tests
{
    public class NonebTagTests
    {
        [Test]
        public void ValidateName()
        {
            Assert.That(NonebTag.IsNameValid(string.Empty, out _), Is.False);
            Assert.That(NonebTag.IsNameValid("   ", out _), Is.False);
            Assert.That(NonebTag.IsNameValid(".NameStartWithDot", out _), Is.False);
            Assert.That(NonebTag.IsNameValid("..", out _), Is.False);
            Assert.That(NonebTag.IsNameValid("NameEndsWithDot.", out _), Is.False);
            Assert.That(NonebTag.IsNameValid("$@InvalidCharacters", out _), Is.False);

            Assert.That(NonebTag.IsNameValid("Name.With.Dot", out _), Is.True);
            Assert.That(NonebTag.IsNameValid("NameWithoutDot", out _), Is.True);
            Assert.That(NonebTag.IsNameValid("AllowedCharacters._123456789", out _), Is.True);
        }

        [Test]
        public void ComparisonTests()
        {
            NonebTag a0 = "Test.A";
            NonebTag a1 = "Test.A";
            NonebTag b = "Test.A.B";

            Assert.IsTrue(a0 == a1);
            Assert.IsTrue(a0 != b);
            Assert.IsFalse(a0 == b);
            Assert.IsFalse(a0.Equals(b));
            Assert.IsTrue(a0.Equals("Test.A"));
        }

        [Test]
        public void IsParentTests()
        {
            NonebTag test = "Test";
            NonebTag a = "Test.A";
            NonebTag b = "Test.A.B";

            Assert.IsTrue(test.IsParentOf(a));
            Assert.IsTrue(test.IsParentOf(b));
            Assert.IsTrue(a.IsParentOf(b));
            Assert.IsTrue(!b.IsParentOf(b));
            Assert.IsTrue(!b.IsParentOf(a));
            Assert.IsTrue(!b.IsParentOf(test));
        }

        [Test]
        public void IsChildOfTests()
        {
            NonebTag test = "Test";
            NonebTag a = "Test.A";
            NonebTag b = "Test.A.B";

            Assert.IsTrue(a.IsChildOf(test));
            Assert.IsTrue(b.IsChildOf(test));
            Assert.IsTrue(b.IsChildOf(a));
            Assert.IsTrue(!b.IsChildOf(b));
            Assert.IsTrue(!a.IsChildOf(b));
            Assert.IsTrue(!test.IsChildOf(b));
        }

        [Test]
        public void ParentTagTests()
        {
            NonebTag test = "Test";
            NonebTag parent = "Test.Parent";
            NonebTag firstChild = "Test.Parent.FirstChild";
            NonebTag secondChild = "Test.Parent.SecondChild";
            NonebTag grandson = "Test.Parent.SecondChild.Grandson";

            CollectionAssert.AreEquivalent(new[] { test, parent }, firstChild.ParentTags.ToArray());
            CollectionAssert.AreEquivalent(new[] { test, parent }, secondChild.ParentTags.ToArray());
            CollectionAssert.AreEquivalent(new[] { test, parent, secondChild }, grandson.ParentTags.ToArray());
        }
    }
}