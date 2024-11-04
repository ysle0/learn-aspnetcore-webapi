using api.DTOs.Comment;
using api.Models;

namespace api.Interfaces;

public interface ICommentRepository {
  ValueTask<Comment?> GetById(int id);
  Task<List<Comment>> GetAll();
  Task<Comment> CreateNew(Comment c);
}