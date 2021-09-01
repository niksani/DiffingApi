using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffingApi.Contexts;
using DiffingApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiffingApi.Services
{
    public class ImagesRepository: IImagesRepository, IDisposable
    {
        private ImageContext _context;

        public ImagesRepository(ImageContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Base64Image> GetAsync(int id)
        {
            return await _context.Images
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public void Update(Base64Image imageToUpdate)
        {
            // no code required, entity tracked by context. 
        }

        public void Add(Base64Image imageToAdd)
        {
            if (imageToAdd == null)
            {
                throw new ArgumentNullException(nameof(imageToAdd));
            }

            _context.Add(imageToAdd);
        }

      
        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

      
    }
}
