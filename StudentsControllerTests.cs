using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentManagement.Controllers;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StudentManagementAPI.UnitTests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> studentServiceStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetStudentAsync_WithUnexistingStudent_ReturnsNotFound()
        {
            // Arrange
            studentServiceStub.Setup(s => s.GetStudentAsync(It.IsAny<string>())).ReturnsAsync((Student?)null);

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.GetStudentAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetStudentAsync_WithExistingStudent_ReturnsExpectedStudent()
        {
            // Arrange
            var expectedStudent = GetRandomStudent();

            studentServiceStub.Setup(s => s.GetStudentAsync(It.IsAny<string>())).ReturnsAsync(expectedStudent);

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.GetStudentAsync(expectedStudent.Id);

            // Assert
            Assert.Equal(expectedStudent, result.Value);
        }

        [Fact]
        public async Task GetAllStudentsAsync_WithExistingStudents_ReturnsAllStudents()
        {
            // Arrange
            var expectedStudents = new[]{ GetRandomStudent(), GetRandomStudent(), GetRandomStudent() };

            studentServiceStub.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(expectedStudents);

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.GetAllStudentsAsync() as ICollection<Student>;

            // Assert
            Assert.Equal(expectedStudents.Length, result?.Count);
        }

        [Fact]
        public async Task CreateStudentAsync_WithStudentToCreate_ReturnsCreatedStudent()
        {
            // Arrange
            var studentToCreate = GetRandomStudent();

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.CreateStudentAsync(studentToCreate);

            // Assert
            var createdStudent = (result.Result as CreatedAtActionResult)?.Value as Student;

            Assert.Equal(studentToCreate, createdStudent);
        }

        [Fact]
        public async Task UpdateStudentAsync_WithExistingStudent_ReturnsNoContent()
        {
            // Arrange
            var existingStudent = GetRandomStudent();

            studentServiceStub.Setup(s => s.GetStudentAsync(It.IsAny<string>())).ReturnsAsync(existingStudent);

            var studentId = existingStudent.Id;

            var studentToUpdate = new Student() { Name = "Alex", Courses = existingStudent.Courses, Age = existingStudent.Age, Gender = existingStudent.Gender, IsGraduated = existingStudent.IsGraduated };

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.UpdateStudentAsync(studentId, studentToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task DeleteStudentAsync_WithExistingStudent_ReturnsNoContent()
        {
            // Arrange
            var existingStudent = GetRandomStudent();

            studentServiceStub.Setup(s => s.GetStudentAsync(It.IsAny<string>())).ReturnsAsync(existingStudent);

            var studentId = existingStudent.Id;

            var controller = new StudentsController(studentServiceStub.Object);

            // Act
            var result = await controller.DeleteStudentAsync(studentId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        private Student GetRandomStudent()
        {
            var testStudent = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Age = rand.Next(18, 50),
                Courses = null,
                Gender = rand.Next(2) == 1 ? "Female" : "Male",
                IsGraduated = rand.Next(2) == 1
            };
            return testStudent;
        }
    }
}