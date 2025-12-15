using POOProject.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POOProject.Models.Repositories.Interfaces
{
    public interface IArranjoRepository
    {
        void SaveArranjo(Arranjo arranjo);
        List<Arranjo> GetAllArranjos();

        void Update(Arranjo arranjo);
    }
}
