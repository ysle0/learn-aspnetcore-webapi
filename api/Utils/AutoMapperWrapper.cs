using api.Controllers;
using api.DTOs.Stock;
using api.Models;
using AutoMapper;

namespace api.Utils;

public static class AutoMapperWrapper {
  public static IMapper RegisterMappings() {
    var cfg = new MapperConfiguration(cfg => {
      cfg.CreateMap<Stock, StockDto>();
      cfg.CreateMap<StockCreateDto, Stock>();
      cfg.CreateMap<StockUpdateDto, Stock>();
    });

    return cfg.CreateMapper();
  }
}