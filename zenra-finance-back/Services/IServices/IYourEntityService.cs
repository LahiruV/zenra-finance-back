using zenra_finance_back.Models;

namespace zenra_finance_back.Services.IServices
{
    public interface IYourEntityService
    {
        IEnumerable<YourEntity> GetAll();
        YourEntity GetById(int id);
        void Create(YourEntity entity);
        void Update(YourEntity entity);
        void Delete(int id);
    }
}
