using Xunit;

namespace MealForToday.Application.Tests
{
    public class SampleTest
    {
        [Fact]
        public void SampleTestScenario()
        {
            var model = new SampleModel();
            Assert.True(model.ShouldReturnTrue());
        }
    }
}
