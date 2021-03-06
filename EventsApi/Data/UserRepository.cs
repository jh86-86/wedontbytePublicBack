using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using System;
using System.Linq;

public class UserRepository : BaseRepository, IRepository<User>
{

  public UserRepository(IConfiguration configuration) : base(configuration) { }

  public async Task<IEnumerable<User>> GetAll()
  {
    using var connection = CreateConnection();
    return await connection.QueryAsync<User>("SELECT * FROM Users;");

  }


  public async Task<User> Get(long id)
  {
    using var connection = CreateConnection();
    return await connection.QuerySingleAsync<User>("SELECT * FROM Users WHERE Id=@Id;", new { Id = id });
  }

  public async Task<User> Insert(User userToInsert)
  {
    using var connection = CreateConnection();
    return await connection.QuerySingleAsync<User>("INSERT INTO Users(FirstName, Surname, Username, Hours, PartOfGroupId, AdminOfGroupId, EventsIds)VALUES (@FirstName, @Surname, @Username, @Hours, @PartOfGroupId, @AdminOfGroupId, @EventsIds)  RETURNING *", userToInsert);
  }
  public async Task<User> Update(User userToUpdate)

  {
    using var connection = CreateConnection();
    return await connection.QuerySingleAsync<User>("UPDATE Users SET FirstName =@FirstName, Surname=@Surname, Username=@Username, Hours=@Hours, PartOfGroupId=@PartOfGroupId, AdminOfGroupId=@AdminOfGroupId, EventsIds=@EventsIds WHERE Id = @Id RETURNING *;", userToUpdate);
  }

  public void Delete(long id)
  {
    using var connection = CreateConnection();
    connection.Execute("DELETE FROM Users WHERE Id = @Id;", new { Id = id });
  }

  public async Task<IEnumerable<User>> Search(string query)
  {
    //check if query passed in is an integer
    using var connection = CreateConnection();
    if (query.All(char.IsDigit))
    {
      //look by groupId
      return await connection.QueryAsync<User>("SELECT * FROM Users WHERE PartOfGroupId = @PartOfGroupId;", new { PartOfGroupId = Int32.Parse(query) });
    }
    //look by username
    return await connection.QueryAsync<User>("SELECT * FROM Users WHERE Username = @Username;", new { Username = query });
  }

}