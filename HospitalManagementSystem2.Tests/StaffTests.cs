using HospitalManagementSystem2.Models.Entities;
using Moq;
using Newtonsoft.Json.Bson;
using System;
using System.Linq.Expressions;
using Xunit;

namespace HospitalManagementSystem2.Tests.Entities
{
    public class StaffTests
    {
        [Fact]
        public void Matches_SameEmail_ReturnsTrue()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = "test@example.com";
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = "test@example.com";

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.True(compiledExpression(comparisonStaff), "Expression should match staff with the same email.");
        }

        [Fact]
        public void Matches_DifferentEmail_ReturnsFalse()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = "test@example.com";
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = "different@example.com";

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.False(compiledExpression(comparisonStaff), "Expression should not match staff with a different email.");
        }

        [Fact]
        public void Matches_CaseInsensitiveEmail_ReturnsTrue()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = "test@example.com";
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = "TEST@example.com";

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.True(compiledExpression(comparisonStaff), "Expression should match staff with case-different email due to case-insensitive comparison.");
        }

        [Fact]
        public void Matches_NullInputEmailAndNonNullComparisonEmail_ReturnsFalse()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = null;
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = "test@example.com";

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.False(compiledExpression(comparisonStaff), "Expression should return false when input email is null and comparison email is non-null.");
        }

        [Fact]
        public void Matches_NullComparisonEmailAndNonNullInputEmail_ReturnsFalse()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = "test@example.com";
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = null;

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.False(compiledExpression(comparisonStaff), "Expression should return false when comparison email is null and input email is non-null.");
        }

        [Fact]
        public void Matches_BothEmailsNull_ReturnsTrue()
        {
            // Arrange
            var staff = new Mock<Staff> { CallBase = true }.Object;
            staff.Email = null;
            var comparisonStaff = new Mock<Staff> { CallBase = true }.Object;
            comparisonStaff.Email = null;

            // Act
            Expression<Func<Staff, bool>> expression = Staff.Matches(staff);
            Func<Staff, bool> compiledExpression = expression.Compile();

            // Assert
            Assert.True(compiledExpression(comparisonStaff), "Expression should match when both emails are null.");
        }
    }
}
