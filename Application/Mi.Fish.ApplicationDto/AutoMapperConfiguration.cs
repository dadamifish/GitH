using System;
using AutoMapper;


namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMapperConfiguration
    {
        public static Action<IMapperConfigurationExpression> Config = configuration =>
        {
            #region SaleDetail

            configuration.CreateMap<GetSaleDetailDto, GetSaleDetailOutput>()
                .ForMember(u => u.BarCode, options => options.MapFrom(input => input.barcode))
                .ForMember(u => u.GoodsCode, options => options.MapFrom(input => input.item_subno))
                .ForMember(u => u.GoodsCode, options => options.NullSubstitute(""))
                .ForMember(u => u.GoodsName, options => options.MapFrom(input => input.item_name))
                .ForMember(u => u.GoodsPrice, options => options.MapFrom(input => input.source_price))
                .ForMember(u => u.PayPrice, options => options.MapFrom(input => input.sale_money))
                .ForMember(u => u.Qty, options => options.MapFrom(input => input.sale_qnty))
                .ForMember(u => u.SaleNo, options => options.MapFrom(input => input.flow_no))
                .ForMember(u => u.SalePrice, options => options.MapFrom(input => input.sale_price))
                .ForMember(u => u.Sort, options => options.MapFrom(input => input.flow_id));

            configuration.
                CreateMap<GetSaleDetailInput, GetSaleDetailProcInput>()
                .ForMember(u => u.flow_no, options => options.NullSubstitute(""))
                .ForMember(u => u.flow_no, options => options.MapFrom(input => input.SaleNo.Trim()))
                .ForMember(u => u.item_name, options => options.NullSubstitute(""))
                .ForMember(u => u.item_name, options => options.MapFrom(input => input.GoodsName.Trim()))
                .ForMember(u => u.item_no, options => options.NullSubstitute(""))
                .ForMember(u => u.item_no, options => options.MapFrom(input => input.BarCode.Trim()))
                .ForMember(u => u.sell_way, options => options.NullSubstitute("-1"))
                .ForMember(u => u.sell_way, options => options.MapFrom(input => Convert.ToString((int)input.SaleType)));

            #endregion

            #region Leyou

            configuration.CreateMap<GetAppGoodsMeal, GetAppGoodsDto>()
            .ForMember(dest => dest.GoodsNo, opt => opt.MapFrom(src => src.MealId))
            .ForMember(dest => dest.GoodsName, opt => opt.MapFrom(src => src.MealName))
            .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.Price.Replace("0000", "")))
            .ForMember(dest => dest.StoreNo, opt => opt.MapFrom(src => src.AssignedDiningStoreId));

            configuration.CreateMap<AddAppGoodsInput, AddAppGoodsApiInput>()
            .ForMember(dest => dest.MealId, opt => opt.MapFrom(src => src.GoodsNo))
            .ForMember(dest => dest.MealName, opt => opt.MapFrom(src => src.GoodsName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.SalePrice));

            #endregion

            #region Category

            configuration.CreateMap<CategoryDTO, CategoryOutPut>()
            .ForMember(u => u.CategoryName, options => options.MapFrom(input => input.Item_Clsname))
            .ForMember(u => u.CategoryNo, options => options.MapFrom(input => input.Item_Clsno))
             .ForMember(u => u.Level, options => options.MapFrom(input => input.Level));

            #endregion

            #region Goods

            configuration.CreateMap<GoodsDTO, GoodsOutPut>()
            .ForMember(u => u.BarCode, options => options.MapFrom(input => input.sGoodTypeID == "C" ? input.item_subno : input.barcode))
            .ForMember(u => u.CostPrice, options => options.MapFrom(input => input.sGoodTypeID == "C" ? 0 : input.cost_price))
            .ForMember(u => u.GoodsNo, options => options.MapFrom(input => input.GoodsID))
            .ForMember(u => u.GoodsName, options => options.MapFrom(input => input.GoodsName))
            .ForMember(u => u.GoodsType, options => options.MapFrom(input => input.sGoodTypeID))
            .ForMember(u => u.Havestork, options => options.MapFrom(input => input.Have_stock))
            .ForMember(u => u.IsGuQing, options => options.MapFrom(input => input.guqingqty >= 0 ? 1 : 0))
            .ForMember(u => u.ParentId, options => options.MapFrom(input => input.sGoodTypeID == "C" ? "-1" : input.GoodsID))
            .ForMember(u => u.SalePrice, options => options.MapFrom(input => input.sale_price))
            .ForMember(u => u.SubItemNo, options => options.MapFrom(input => input.item_subno));

            #endregion

            #region AppGoods

            configuration.CreateMap<MealList, MealListOutputDto>().ForMember(dest => dest.StoreId, opt => opt.MapFrom(source => source.AssignedDiningStoreId));
            configuration.CreateMap<AddAppGoodsInputDto, AddAppGoodsToAppInputDto>();
            configuration.CreateMap<OrdersPrintOutputDto, OrdersPrintDto>();
            configuration.CreateMap<OrderMealPrint, Good>();
            #endregion

            #region Order Detail

            configuration.CreateMap<OrderDetail, OrderGoods>()
            .ForMember(u => u.DishNo, options => options.MapFrom(input => input.dishno))
            .ForMember(u => u.FlowId, options => options.MapFrom(input => input.flow_id))
            .ForMember(u => u.GoodsID, options => options.MapFrom(input => input.item_subno))
            .ForMember(u => u.GoodsName, options => options.MapFrom(input => input.item_name))
            .ForMember(u => u.GoodsPrice, options => options.MapFrom(input => input.orderprice))
            .ForMember(u => u.Quantity, options => options.MapFrom(input => input.sale_qnty));

            configuration.CreateMap<OrderpayDetail, OrderPayDetailOutput>()
           .ForMember(u => u.Payway, options => options.MapFrom(input => input.pay_way))
           .ForMember(u => u.Saleamount, options => options.MapFrom(input => input.sale_amount));

            configuration.CreateMap<OrderSaleListDto, OrderSaleListOutPut>()
                .ForMember(u => u.FlowNo, options => options.MapFrom(input => input.flow_no));

            #endregion

            #region User

            configuration.CreateMap<UserLoginInput, UserLoginDto>()
                .ForMember(u => u.cashierid, options => options.MapFrom(input => input.CashierNo))
                .ForMember(u => u.cashierpw, options => options.MapFrom(input => input.CashierPwd))
                .ForMember(u => u.startamount, options => options.MapFrom(input => input.StartAmount))
                .ForMember(u => u.banci, options => options.MapFrom(input => input.Shift));

            configuration.CreateMap<UserPwdUpdateInput, UserPwdUpdateDto>()
                .ForMember(u => u.oldpwd, options => options.MapFrom(input => input.OldPwd))
                .ForMember(u => u.newpwd, options => options.MapFrom(input => input.NewPwd));

            #endregion

            #region Test

            configuration.CreateMap<AreaInfo, AreaInfoOutput>()
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(source => source.area_name))
                .ForMember(dest => dest.AreaNo, opt => opt.MapFrom(source => source.area_no))
                .ForMember(dest => dest.InputTime, opt => opt.MapFrom(source => source.inputtime))
                .ForMember(dest => dest.InputBy, opt => opt.MapFrom(source => source.inputby))
                .ForMember(dest => dest.LastUpdateTime, opt => opt.MapFrom(source => source.dLastUpdateTime));


            #endregion

            #region 下单打印输出

            configuration.CreateMap<PrintInfo, OrdersPrintDto>()
                .ForMember(dest => dest.Goods, opt => opt.MapFrom(source => source.Menus));

            configuration.CreateMap<GoodsInfo, Good>()
                .ForMember(dest => dest.GoodSubs, opt => opt.MapFrom(source => source.MealSubs))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.PayAmount))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(source => source.SalePrice));

            configuration.CreateMap<MealSubInfo, GoodSub>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.PayAmount))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(source => source.SalePrice));

            configuration.CreateMap<RePrintDto, OrdersPrintDto>()
                .ForMember(dest => dest.PayableAmount, opt => opt.MapFrom(source => source.PayAbleAmount.ToString("N2")))
                .ForMember(dest => dest.GiveAmount, opt => opt.MapFrom(source => source.ReturnMoney));

            configuration.CreateMap<RePrintGoodsDto, Good>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(source => source.PayPrice));

            configuration.CreateMap<OrderDetail, GoodsPrint>()
               .ForMember(dest => dest.item_name, opt => opt.MapFrom(source => source.item_name))
               .ForMember(dest => dest.sale_qnty, opt => opt.MapFrom(source => source.sale_qnty));

            #endregion

        };
    }
}
