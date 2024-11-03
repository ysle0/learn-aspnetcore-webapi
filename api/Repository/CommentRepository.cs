using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class CommentRepository(
  MySqlContext _c,
  IMapper _mp
) : ICommentRepository {
  public async ValueTask<Comment?> GetById(int id) =>
    await _c.Comments.FindAsync(id);

  public async Task<List<Comment>> GetAll() => await _c.Comments.ToListAsync();

  public async Task<Comment> CreateNew(Comment c) {
    await _c.Comments.AddAsync(c);
    await _c.SaveChangesAsync();

    return c;
  }
}