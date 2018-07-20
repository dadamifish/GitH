namespace Mi.Fish.Application
{
    public class ApplicationConsts
    {
        public const string QueryOrderDetail = @"select t2.sGoodsNO as dishno,cast(t1.nSaleAmount/t1.nSaleQty  as   numeric(6,2)) as orderprice, 
                                                 nItem as flow_id,nSerID as flow_no,t2.sGoodsName as item_name,t2.sMainBarcode as bbarcode,nSaleQty as sale_qnty,
                                                 nSaleAmount as sale_money,t2.sGoodsNO as item_subno,null as deskno,null as vipno,t1.dTradeDate as oper_date
                                                 from tFishSaleDtl t1 inner join tGoods t2 on t1.nGoodsID=t2.nGoodsID 
                                                 where t1.sStoreNO = @StoreNO and t1.sFishNO = @FishNo and t1.nSerID = @orderNo
                                                 and dTradeDate>=Convert(varchar(10),getdate(),120)  and dTradeDate<Convert(varchar(10),getdate()+1,120)";

        public const string QueryOrderPayList = @"select a.sPayTypeID as pay_way,a.nItem as sell_way,a.nPaytAmount as old_amount,
                          a.nPaytAmount as sale_amount,a.sPaytNO as thirdtradeno ,b.tradeno  from tFishPayt a 
                          left join ft_thirdpayment b on a.sPaytNO=b.out_trade_no and a.sStoreNO=b.branch_no 
                          and a.sFishNO=b.jh where a.sStoreNO = @StoreNO and a.sFishNO =@FishNo and a.nSerID = @orderNo
                          and a.dTradeDate>=Convert(varchar(10),getdate(),120) and a.dTradeDate<Convert(varchar(10),getdate()+1,120)";
        public const string QueryTopCategory = @"SELECT sCategoryNO as Item_Clsno ,sCategoryDesc as Item_Clsname,1 as Level from tCategory  
                                                WHERE sCategoryNO in 
                                                (
                                               	select distinct substring(sPath,dbo.fn_find('/',sPath,2)+1,dbo.fn_find('/',sPath,2+1)-dbo.fn_find('/',sPath,2)-1) as item_clsno  
                                              	from  base_entry_message_allow b left join ft_v_SaleGoods a on a.sGoodsNO=b.item_no 
                                                where b.activeflag=1 and b.branch_no=@storageno and b.jh=@terminalid  
                                                union all (select distinct convert(varchar(4),nCategoryID) as item_clsno from tCategory where sCategoryDesc=@promotionname)
                                                ) order by item_clsno";
        public const string QuerySubLevelCategory = @"SELECT sCategoryNO as Item_Clsno ,sCategoryDesc as Item_Clsname,@jibie-1 as Level from tCategory  
                          WHERE sCategoryNO in 
                          (
                          	select distinct substring(sPath,dbo.fn_find('/',sPath,@jibie)+1,dbo.fn_find('/',sPath,@jibie+1)-dbo.fn_find('/',sPath,@jibie)-1) as item_clsno  
                              from  base_entry_message_allow b left join ft_v_SaleGoods a on a.sGoodsNO=b.item_no 
                          	where b.activeflag=1 and b.branch_no=@storageno and b.jh=@terminalid and substring(a.sPath,dbo.fn_find('/',a.sPath,@jibie-1)+1,dbo.fn_find('/',a.sPath,@jibie)-dbo.fn_find('/',a.sPath,@jibie-1)-1) = @shangji
                          	union all (select distinct convert(varchar(4),nCategoryID) as item_clsno from tCategory where sCategoryDesc=@promotionname)
                          )  order by item_clsno";
        public const string QueryGoodsListByCategory= @"select sGoodsNO as GoodsID, a.sGoodsName as GoodsName, 
                          a.Have_stock, a.nRealSalePrice as sale_price, b.guqingqty,
                          a.sProductCode as item_subno, 
                          a.cost_price, sMainBarcode as barcode 
                          from base_entry_message_allow b  left join ft_v_SaleGoods a
                          on a.sGoodsNO=b.item_no  
	                      where b.activeflag=1 and b.branch_no=@storageno and b.jh=@terminalid
                          and substring(a.sPath,dbo.fn_find('/',a.sPath,4)+1,dbo.fn_find('/',a.sPath,4+1)-dbo.fn_find('/',a.sPath,4)-1) =@categoy
	                      order by a.sGoodsNO ";
        public const string QueryCustomerGoodsList= @"select a.sGoodsNO as GoodsID,a.sGoodsName as GoodsName,a.Have_stock, 
	                      a.nRealSalePrice as sale_price, guqingqty,a.sProductCode as item_subno,a.sMainBarcode as barcode,
	                      ticketVolume,a.cost_price,a.sGoodTypeID from ft_v_SaleGoods a
	                      inner join base_entry_message_allow c on a.sGoodsNO=c.item_no 
	                      where c.sortorder>0 and c.branch_no=@storageno and c.jh=@terminalid and c.activeflag=1 and a.sGoodsNO <> '0' and sStoreNO= @storageno  ORDER BY c.sortorder ASC";
        public const string QueryHotGoodsList= @"SELECT top 20 a.SaleCount,b.sGoodsNO as GoodsID,b.sGoodsName as GoodsName,
	                      b.sProductCode as item_subno,b.have_stock as Have_stock,b.nRealSalePrice as sale_price,-1 as guqingqty,
	                      b.sGoodTypeID as sGoodTypeID,b.cost_price as cost_price,b.sMainBarcode as barcode from 
	                      (
	                      	select count(*) as SaleCount,GoodsID from PurchaseItem 
	                      	where BuyDate = Convert(varchar,getdate(),112) group by GoodsID 
	                      ) a left join ft_v_SaleGoods b on a.GoodsID = B.nGoodsID
	                      order by a.SaleCount desc ";

        public const string QueryGoodsByKey = @"select a.sGoodsNO as ItemNo,isnull(a.sMainBarcode,a.sGoodsNO) as GoodsNo,a.sGoodsName as GoodsName 
                            from ft_v_SaleGoods a inner join base_entry_message_allow b on a.sGoodsNO=b.item_no where 
                            b.branch_no = @storageno and jh=@terminalid and a.sStoreNO=@storageno";

        public const string QueryLastSaleList =
            "select top 10 nSerID as flow_no from tFishSale where sStoreNO = @StoreNO and sFishNO = @FishNO and sCashierNO1 = @CashierNO " +
            " and nTradeType = 0 and dTradeDate>=Convert(varchar(10),getdate(),120) and dTradeDate<Convert(varchar(10),getdate()+1,120) " +
            " and nSerID not in ( select nSerID from ft_return_order where sStoreNO =@StoreNO " +
            " and sFishNO =@FishNO and dTradeDate>=Convert(varchar(10),getdate(),120) and dTradeDate<Convert(varchar(10),getdate()+1,120) " +
            " union select nSerID from tFishPayt where sPayTypeID = '105' and dTradeDate>=Convert(varchar(10),getdate(),120) and dTradeDate<Convert(varchar(10),getdate()+1,120) and sStoreNO = @StoreNO and sFishNO = @FishNO )" +  //过滤app退单
            " order by dTradeDate desc";

        public const string QueryRefundOrder= @"select count(*) from ft_return_order where sStoreNO =@StoreNO  and sFishNO =@FishNo and nSerID = @orderNo
                                            and dTradeDate>=Convert(varchar(10),getdate(),120)  and dTradeDate<Convert(varchar(10),getdate()+1,120)";
        public const string QueryRefund = @"select count(*) from tFishSale where sStoreNO =@StoreNO and sFishNO = @FishNo and nSerID = @orderNo 
                                            and dTradeDate>=Convert(varchar(10),getdate(),120) and dTradeDate<Convert(varchar(10),getdate()+1,120) and nTradeType=2";
    }
}
