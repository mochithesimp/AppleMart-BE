using iPhoneBE.Data.Model;
using iPhoneBE.Data.Models.AttributeModel;

namespace iPhoneBE.Service.Interfaces
{
    public interface IAttributeServices
    {
        Task<Data.Entities.Attribute> AddAsync(Data.Entities.Attribute attribute);
        Task<Data.Entities.Attribute> DeleteAsync(int id);
        Task<IEnumerable<Data.Entities.Attribute>> GetAllAsync();
        Task<Data.Entities.Attribute> GetByIdAsync(int id);
        Task<Data.Entities.Attribute> UpdateAsync(int id, UpdateAttributeModel newAttribute);
    }
}