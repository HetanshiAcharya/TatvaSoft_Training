using AssignmentEntity.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentEntity.ViewModels
{
    public class ProjectDetails
    {
        public int? Id { get; set; }

        public string ProjectName { get; set; } = null!;

        public string? Assignee { get; set; }

        public int? DomainId { get; set; }

        public string? Description { get; set; }

        public TimeOnly? DueDate { get; set; }

        public string? Domain { get; set; }

        public string? City { get; set; }

    }
}
