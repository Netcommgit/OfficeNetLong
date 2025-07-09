using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.Thought
{
    public interface IThoughtService
    {
        public Task<ThoughtSaveModel> SaveThought(ThoughtSaveModel thoughtOfDay);
        public Task<List<ThoughtSaveModel>> GetThoughtOfTheDay(bool Flag);
    }
}
