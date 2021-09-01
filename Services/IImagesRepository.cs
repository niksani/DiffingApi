using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffingApi.Entities;

namespace DiffingApi.Services
{
    public interface IImagesRepository
    {
        Task<List<Base64Image>> GetAll();

        Task<Base64Image> GetAsync(int id);

        void Update(Base64Image imageToUpdate);

        void Add(Base64Image imageToAdd);

        Task<bool> SaveChangesAsync();
    }
}
