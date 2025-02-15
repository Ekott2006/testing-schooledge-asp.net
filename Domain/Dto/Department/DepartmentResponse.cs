using Domain.Dto.Course;
using Domain.Dto.Faculty;

namespace Domain.Dto.Department;

public class DepartmentResponse()
{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FacultyId { get; set; }
        public ICollection<CourseResponse> Courses { get; set; } = [];
 
        public DepartmentResponse(Model.Department? department): this()
        {
                if (department == null) return;
                Name = department.Name;
                FacultyId = department.FacultyId;
                Id = department.Id;
                Courses = department.Courses.Select(x => new CourseResponse(x)).ToList();
        }
}