using InventoryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly string _connectionsString;
        
        public CategoriesService(string connectionsString)
        {
            this._connectionsString = connectionsString;
        }

        void ICategoriesService.AddCategory(string categoryName, List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }
        
        void ICategoriesService.DeleteCategory(string categoryName, List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }

        ItemCategory ICategoriesService.GetCategories(List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }

        void ICategoriesService.UpdateCategory(string categoryName, List<ItemCategory> categories)
        {
            throw new NotImplementedException();
        }
    }
}
