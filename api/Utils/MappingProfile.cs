using api.Controllers;
using api.DTOs.Comment;
using api.DTOs.Stock;
using api.Models;
using AutoMapper;

namespace api.Utils;

public class MappingProfile : Profile {
  public MappingProfile() {
    CreateMap<Stock, StockDto>();
      // .IncludeMembers(s => s.Comments)
      // .ForMember(dst => dst.)
      
    CreateMap<StockCreateDto, Stock>();
    CreateMap<StockUpdateDto, Stock>();

    CreateMap<Comment, CommentDto>();
    CreateMap<CreateCommentDto, Comment>();
    CreateMap<UpdateCommentDto, Comment>();
  }
}