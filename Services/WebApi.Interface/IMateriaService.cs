using WebApi.Model;

namespace WebApi.Interface;

public interface IMateriaService
{
    /*public MateriaEntities Add(MateriaEntities materia);
    public IEnumerable<MateriaEntities> GetALL();
    public MateriaEntities GetByID(int id);
    void Update(MateriaEntities materia);
    void Delete(int id);*/

    public List<MateriaEntities> GetAll();
    public Task<MateriaEntities> Add(MateriaEntities materia);
    public Task<MateriaEntities> GetById(int id);
    public Task<MateriaEntities> Update(MateriaEntities materia);
    public Task<int> Delete(int id);
}
