using NetCoreUnitTest.App;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCoreUnitTest.Test
{
    public class CalculatorTest
    {
        [Fact]
        public void AddTest() {

            //arrange -Initialize
            //int a = 5;
            //int b = 20;
            //var calculater = new Calculator();

            ////act
            //var total = calculater.Add(a, b);

            ////assert -Dogrulama(Test)
            //Assert.Equal<int>(25, total);

            //Beklenen ve aranacak string degerleri alıyor
            //Assert.Contains("Zeynep", "Zeynep Öz.");
            //Assert.DoesNotContain("Zeynepp", "Zeynep Öz.");

            //var names = new List<string> { "zeynep", "özt.", "ali" };
            //Assert.Contains(names, x => x == "zeynep");

            //tip karsılastirmasi
            //Assert.Tru /False metodları ile condition if gibi kosulları karsilastirabiliriz.
            //Assert.True("zeynep".GetType() == typeof(string));

            //Kelimenin 'dog' ile basladıgının kontrolü
            var regex = "^dog";
            Assert.Matches(regex, "dog aa");

        }
    }
}
