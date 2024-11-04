using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class CommentRepository : ICommentRepository {
  readonly MySqlContext _ctx;
  readonly IMapper _mapper;

  public CommentRepository(MySqlContext context, IMapper mapper) {
    _ctx = context;
    _mapper = mapper;
  }

  public async ValueTask<Comment?> GetById(int id) =>
    await _ctx.Comments.FindAsync(id);

  public async Task<List<Comment>> GetAll() => await _ctx.Comments.ToListAsync();

  public async Task<Comment> CreateNew(Comment c) {
    await _ctx.Comments.AddAsync(c);
    await _ctx.SaveChangesAsync();

    return c;
  }
}