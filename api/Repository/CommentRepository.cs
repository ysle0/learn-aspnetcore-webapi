using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using DbContext = api.Data.DbContext;

namespace api.Repository;

public class CommentRepository : ICommentRepository {
  readonly DbContext _ctx;
  readonly IMapper _mapper;

  public CommentRepository(DbContext context, IMapper mapper) {
    _ctx = context;
    _mapper = mapper;
  }

  public async ValueTask<Comment?> GetById(int id) {
    return await _ctx.Comments.FindAsync(id);
  }

  public async Task<List<Comment>> GetAll() {
    return await _ctx.Comments.ToListAsync();
  }

  public async Task<Comment> CreateNew(Comment comment) {
    await _ctx.Comments.AddAsync(comment);
    await _ctx.SaveChangesAsync();

    return comment;
  }

  public async Task<Comment?> Update(int id, Comment comment) {
    var existingComment = await _ctx.Comments.FindAsync(id);
    if (existingComment == null) return null;

    existingComment.Title = comment.Title;
    existingComment.Content = comment.Content;

    await _ctx.SaveChangesAsync();

    return existingComment;
  }

  public async Task<Comment?> Delete(int id) {
    var existingComment
      = await _ctx.Comments.FirstOrDefaultAsync(e => e.Id == id);
    if (existingComment == null) return null;

    _ctx.Remove(existingComment);
    await _ctx.SaveChangesAsync();

    return existingComment;
  }
}