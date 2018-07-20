using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Castle.Core.Logging;


namespace Mi.Fish.Application
{
    /// <summary>
    /// 查询系统前后台销售数据是否一致
    /// </summary>
    public class CheckSyncData
    {
     
        private readonly LocalDbContext _localDbContext;
        private readonly ParkDbContext _parkDbContext;
        private readonly UserData _userData;

        public CheckSyncData(LocalDbContext localDbContext, ParkDbContext parkDbContext,UserData userData)
        {
            _localDbContext = localDbContext;
            _parkDbContext = parkDbContext;
            _userData = userData;
        }


        //判断前后台对应表数据是否一致
        public  async Task<bool> IsDataSyncSuccess(string receiptID)
        {
            double sum_purchase = 0;     //前台销售总金额
            double sum_purchaseitem = 0;   //前台销售明细总金额
            double sum_payment = 0;        //前台支付表金额
            double sum_tpossale = 0;     //后台销售总金额
            double sum_tpossaledtl = 0;    //后台销售明细总金额
            double sum_tpospay = 0;        //后台支付表金额


            sum_purchase =await GetSum_Purchase(receiptID);
            sum_purchaseitem =await GetSum_PurchaseItem(receiptID);
            sum_payment = await GetSum_payment(receiptID);
            sum_tpossale = await GetSum_FishSale(receiptID);
            sum_tpossaledtl = await GetSum_FishSaleDtl(receiptID);
            sum_tpospay = await GetSum_FishPayt(receiptID);

            //1、前后台数据一致，无需同步。2、sum_Purchase为0,无有效销售数据，无需数据同步
            if ((sum_purchase == sum_tpossale && sum_purchaseitem == sum_tpossaledtl && sum_payment == sum_tpospay) || sum_purchase == 0)
            {
                return true;
            }
            return false;
        }

        //前后台数据不一致，更新前台Purchase表时间,调用同步程序同步数据
        public async Task<bool>  UpdateTB_purchase(string receiptID)
        {
            //更新前台Purchase表时间,调用同步程序同步数据
            string sql = !string.IsNullOrEmpty(receiptID) ? string.Format("update Purchase set dLastUpdateTime=dateadd(mi,3,GETDATE()) where BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and ReceiptID = '{0}' and Status=2", receiptID) 
                : string.Format("update Purchase set dLastUpdateTime=dateadd(mi,3,GETDATE()) where BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and Status=2");

            int result = await _localDbContext.ExecuteNonQueryAsync(sql, null);
            return result > 0;
        }



        //获取前台销售表总金额
        private async Task<double>  GetSum_Purchase(string receiptID)
        {
            double sum_purchase = 0;
            string sql = !string.IsNullOrEmpty(receiptID) ? string.Format("select isnull(sum(TotalAmount), 0) as TotalAmount from Purchase where BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and ReceiptID = '{0}' and Status=2 ", receiptID)  //查询上一笔
                : string.Format("select isnull(sum(TotalAmount), 0) as TotalAmount from Purchase where BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and Status=2 "); //退出前查询当天所有

            object getObject =await _localDbContext.ExecuteScalarAsync(sql,null);
            if (getObject!=null)
            {
                sum_purchase = Convert.ToDouble(getObject.ToString());
            }
     
            return sum_purchase;
        }

        //获取前台销售明细表总金额
        private async Task<double> GetSum_PurchaseItem(string receiptID)
        {
            double sum_purchaseitem = 0;
            string sql = !string.IsNullOrEmpty(receiptID) ? string.Format("select isnull(sum(a.Amount), 0) as Amount from PurchaseItem a inner join Purchase b on a.BuyDate=b.BuyDate and a.ReceiptID=b.ReceiptID where a.BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and a.ReceiptID = '{0}' and b.Status=2 ", receiptID)          //查询上一笔
                : string.Format("select isnull(sum(a.Amount), 0) as Amount from PurchaseItem a inner join Purchase b on a.BuyDate=b.BuyDate and a.ReceiptID=b.ReceiptID where a.BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and b.Status=2 ");//退出前查询当天所有
            object getObject = await _localDbContext.ExecuteScalarAsync(sql, null);
            if (getObject!=null)
            {
              sum_purchaseitem = Convert.ToDouble(getObject.ToString());
            }

            return sum_purchaseitem;
        }

        //获取前台支付表金额
        private async Task<double> GetSum_payment(string receiptID)
        {
            double sum_payment = 0;
            string  sql = !string.IsNullOrEmpty(receiptID) ? string.Format("select isnull(sum(a.PayAmount), 0) as PayAmount from Payment a inner join Purchase b on a.BuyDate=b.BuyDate and a.ReceiptID=b.ReceiptID where a.BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and a.ReceiptID = '{0}' and b.Status=2 ", receiptID)//    //查询上一笔
                : string.Format("select isnull(sum(a.PayAmount), 0) as PayAmount from Payment a inner join Purchase b on a.BuyDate=b.BuyDate and a.ReceiptID=b.ReceiptID where a.BuyDate=convert(datetime,convert(varchar(10),getdate(),120)) and b.Status=2 "); //退出前查询当天所有

            object getObject = await _localDbContext.ExecuteScalarAsync(sql, null);
            if (getObject!=null)
            {
                sum_payment = Convert.ToDouble(getObject.ToString());
            }

            return sum_payment;
        }

        //获取后台销售表总金额
        private async Task<double> GetSum_FishSale(string receiptID)
        {
            double sum_tpossale = 0;
            string sql = string.Empty;
            //查询上一笔
            if (!string.IsNullOrEmpty(receiptID))
            {
                int nSerID = int.Parse(receiptID.Remove(0, receiptID.Length - 4));
                sql =
                    $"select isnull(sum(nSaleAmount), 0) as SaleAmount from tFishSale where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and nSerID = '{nSerID.ToString()}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120)";
            }
            //退出前查询当天所有
            else
            {
                sql =$"select isnull(sum(nSaleAmount), 0) as SaleAmount from tFishSale where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120) ";
            }

            object getObject = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (getObject != null)
            {
                sum_tpossale = Convert.ToDouble(getObject.ToString());
            }

            return sum_tpossale;
        }

        //获取后台销售明细表总金额
        private async Task<double> GetSum_FishSaleDtl(string receiptID)
        {
            double sum_tpossaledtl = 0;
            string sql = string.Empty;
            //查询上一笔
            if (!string.IsNullOrEmpty(receiptID))
            {
                int nSerID = int.Parse(receiptID.Remove(0, receiptID.Length - 4));
                sql =
                    $"select isnull(sum(nSaleAmount), 0) as SaleAmount from tFishSaleDtl where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and nSerID = '{nSerID.ToString()}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120) ";
            }
            //退出前查询当天所有
            else
            {
                sql =
                    $"select isnull(sum(nSaleAmount), 0) as SaleAmount from tFishSaleDtl where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120)";
            }

            object getObject = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (getObject != null)
            {
               sum_tpossaledtl = Convert.ToDouble(getObject.ToString());
            }

            return sum_tpossaledtl;
        }

        //获取后台支付表金额
        private async Task<double>  GetSum_FishPayt(string receiptID)
        {
            double sum_tpospayt = 0;
            string sql = string.Empty;
            //查询上一笔
            if (!string.IsNullOrEmpty(receiptID))
            {
              int nSerID = int.Parse(receiptID.Remove(0, receiptID.Length - 4));
              sql =
                  $"select isnull(sum(nPaytAmount), 0) as PayAmount from tFishPayt where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and nSerID = '{nSerID.ToString()}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120) ";
            }
            //退出前查询当天所有
            else
            sql =
                $"select isnull(sum(nPaytAmount), 0) as PayAmount from tFishPayt where sStoreNO='{_userData.StorageNo}' and sFishNO = '{_userData.TerminalID}' and convert(varchar(10),dTradeDate,120)=convert(varchar(10),getdate(),120)";

            object getObject = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (getObject != null)
            {
              sum_tpospayt = Convert.ToDouble(getObject.ToString());
            }

            return sum_tpospayt;
        }
    }
}
