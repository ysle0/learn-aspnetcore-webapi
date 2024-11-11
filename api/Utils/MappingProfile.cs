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

    CreateMap<RegisterUserDto, NewUserDto>()
      .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.EmailAddress));
    CreateMap<LoginDto, NewUserDto>();


    CreateMap<Portfolio, Stock>()
      .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Stock.Id))
      .ForMember(dst => dst.Symbol, opt => opt.MapFrom(src => src.Stock.Symbol))
      .ForMember(dst => dst.CompanyName,
        opt => opt.MapFrom(src => src.Stock.CompanyName))
      .ForMember(dst => dst.Purchase,
        opt => opt.MapFrom(src => src.Stock.Purchase))
      .ForMember(dst => dst.LastDividend,
        opt => opt.MapFrom(src => src.Stock.LastDividend))
      .ForMember(dst => dst.Industry,
        opt => opt.MapFrom(src => src.Stock.Industry))
      .ForMember(dst => dst.MarketCap,
        opt => opt.MapFrom(src => src.Stock.MarketCap));
  }
}