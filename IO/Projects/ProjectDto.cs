using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO._shared;
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.IO.Projects
{
    public class ProjectDto : BaseDto<Project>
    {
        public ProjectDto()
        {

        }
        public ProjectDto(Project entity) : base(entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            Name = entity.Name;
            Description = entity.Description;
            Notes = entity.Notes;
            Status = entity.Status;
            ClientId = entity.ClientId;
            Client = entity.Client;
            Tarefas = entity.Tarefas.Select(t => new TarefaDto(t)).ToList();
            ApplicationUserId = entity.ApplicationUserId;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Created;
        public Guid? ClientId { get; set; }
        public string ApplicationUserId { get; set; }
        public Client Client { get; set; }
        public List<TarefaDto> Tarefas { get; set; } = new();


    }
}