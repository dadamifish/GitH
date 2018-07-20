using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 用户配置数据缓存类
    /// </summary>
    public class UserData
    {
        private  string m_strUserID = string.Empty;

        private  string m_strUserName = string.Empty;

        private  string m_nTerminalID = string.Empty;

        private  string m_strStorage = string.Empty;

        private  string m_strStorageName = string.Empty;

        private  string m_CurrencyEN = string.Empty;

        private  string m_CurrencyCN = string.Empty;

        private  string m_WorkGroup = string.Empty;

        private  string m_print_cash_port = string.Empty;

        private  string m_tel = string.Empty;

        private  string m_lastprint = string.Empty;

        private  bool m_print = true;

        private  string mutonpayRole = string.Empty;
        private  string m_applink = string.Empty;
        private  string m_paylink = string.Empty;
        private  string m_mutonpaylink = string.Empty;
        private  string m_leyoupayLink = string.Empty;

        private string m_syncDataPath = @"D:\APOSV7\Release";

        /// <summary>
        /// 同步程序地址
        /// </summary>
        public string SyncDataPath
        {
            get
            {
                return m_syncDataPath;
            }
            set
            {
                m_syncDataPath = value;
            }
        }

        private string m_FishDataSyncPath = @"D:\APOSV7\SyncData.exe";

        /// <summary>
        /// 智盟同步程序地址
        /// </summary>
        public string FishDataSyncPath
        {
            get
            {
                return m_FishDataSyncPath;
            }
            set
            {
                m_FishDataSyncPath = value;
            }
        }


        private  string m_mutonparkid = string.Empty;
        private  string m_postest = "1";

        private  string m_mutonePrice = "1";

        /// <summary>
        /// 秒通支付取价  1 默认价格 0 实际售价
        /// </summary>
        public  string MutonePrice
        {
            get
            {
                return m_mutonePrice;
            }
            set
            {
                m_mutonePrice = value;
            }
        }


        private  string m_leyouPrice = "1";
        /// <summary>
        /// 方特支付取价  1 默认价格 0 实际售价
        /// </summary>
        public  string LeyouPrice
        {
            get
            {
                return m_leyouPrice;
            }
            set
            {
                m_leyouPrice = value;
            }
        }
        

        private string m_volumerPayParkId = "";
        /// <summary>
        /// 礼券支付ParkId
        /// </summary>
        public string VolumePayParkId
        {
            get
            {
                return m_volumerPayParkId;
            }
            set
            {
                m_volumerPayParkId = value;
            }
        }

        private string m_volumePayLink = "";
        /// <summary>
        /// 礼券支付接口地址
        /// </summary>
        public string VolumePayLink
        {
            get
            {
                return m_volumePayLink;
            }
            set
            {
                m_volumePayLink = value;
            }
        }


        /// <summary>
        /// 方特接口帐号
        /// </summary>
        public string LeyouAccount { get; set; }

        /// <summary>
        /// 方特接口密码
        /// </summary>
        public  string LeyouPwd { get; set; }

        /// <summary>
        /// 方特Token
        /// </summary>
        public  string leyouToken;

        public  string StartLeyouTips;
        public  bool leyouTipsShow;
        public  int leyouNewOrderTimes;

        /// <summary>
        /// 当前点击商品
        /// </summary>
        public  string good;
        /// <summary>
        /// 数量
        /// </summary>
        public  string shuliang;
        /// <summary>
        /// 单价
        /// </summary>
        public  string danjia;
        /// <summary>
        /// 小计
        /// </summary>
        public  string xiaoji;
        /// <summary>
        /// 是否新开单
        /// </summary>
        public  bool newOrder = false;

        /// <summary>
        /// 是否启动手机订餐自动打印功能
        /// </summary>
        public  bool PhonePrintStart = false;

        /// <summary>
        /// 找零金额
        /// </summary>
        public  string zhaoling;
        /// <summary>
        /// 总金额
        /// </summary>
        public  string allAmount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public  string shishou;
        /// <summary>
        /// 应收金额
        /// </summary>
        public  string yingshou;

        /// <summary>
        /// 优惠金额
        /// </summary>
        public  string youhui;
        /// <summary>
        /// 商品详情
        /// </summary>
        public  string[] goodsdetail;

        private  string m_MutoneKey = string.Empty;
        public  string MutoneKey
        {
            get
            {
                return m_MutoneKey;
            }
            set
            {
                m_MutoneKey = value;
            }
        }

        private  string m_LeyouKey = string.Empty;
        public  string LeyouKey
        {
            get
            {
                return m_LeyouKey;
            }
            set
            {
                m_LeyouKey = value;
            }
        }

        public  string Postest
        {
            get
            {
                return m_postest;
            }
            set
            {
                m_postest = value;
            }
        }



        public  bool Print
        {
            get
            {
                return m_print;
            }
            set
            {
                m_print = value;
            }
        }

        /// <summary>
        /// 秒通公园ID
        /// </summary>
        public  string MutonParkid
        {
            get
            {
                return m_mutonparkid;
            }
            set
            {
                m_mutonparkid = value;
            }
        }
        public  string MutonpayRole
        {
            get
            {
                return mutonpayRole;
            }
            set
            {
                mutonpayRole = value;
            }
        }


        /// <summary>
        /// APP地址
        /// </summary>
        public  string AppLink
        {
            get
            {
                return m_applink;
            }
            set
            {
                m_applink = value;
            }
        }

        /// <summary>
        /// 支付地址
        /// </summary>
        public string PayLink { get; set; }

        /// <summary>
        /// 秒通支付地址
        /// </summary>
        public string MutonpayLink { get; set; }

        /// <summary>
        /// 方特支付地址
        /// </summary>
        public  string LeyoupayLink { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public  string UserID
        {
            get
            {
                return m_strUserID;
            }
            set
            {
                m_strUserID = value;
            }
        }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public  string UserName
        {
            get
            {
                return m_strUserName;
            }
            set
            {
                m_strUserName = value;
            }
        }

        /// <summary>
        /// 机号
        /// </summary>
        public  string TerminalID
        {
            get
            {
                return m_nTerminalID;
            }
            set
            {
                m_nTerminalID = value;
            }
        }





        private  string webkey = string.Empty;
        /// <summary>
        /// 第三方支付密钥
        /// </summary>
        public  string WebKey
        {
            get
            {
                return webkey;
            }
            set
            {
                webkey = value;
            }
        }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public  string StorageName
        {
            get
            {
                return m_strStorageName;
            }
            set
            {
                m_strStorageName = value;
            }
        }


        /// <summary>
        /// 门店号
        /// </summary>
        public  string StorageNo
        {
            get
            {
                return m_strStorage;
            }
            set
            {
                m_strStorage = value;
            }
        }



        /// <summary>
        /// 币种（英文）
        /// </summary>
        public  string CurrencyEN
        {
            get
            {
                return m_CurrencyEN;
            }
            set
            {
                m_CurrencyEN = value;
            }
        }

        /// <summary>
        /// 币种（中文）
        /// </summary>
        public  string CurrencyCN
        {
            get
            {
                return m_CurrencyCN;
            }
            set
            {
                m_CurrencyCN = value;
            }
        }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal CoinRate { get; set; }


        /// <summary>
        /// 班次
        /// </summary>
        public  string WorkGroup
        {
            get
            {
                return m_WorkGroup;
            }
            set
            {
                m_WorkGroup = value;
            }
        }

        /// <summary>
        /// 打印机钱箱端口
        /// </summary>
        public  string PrintCashPort
        {
            get
            {
                return m_print_cash_port;
            }
            set
            {
                m_print_cash_port = value;
            }
        }


        /// <summary>
        /// 餐厅电话
        /// </summary>
        public  string Tel
        {
            get
            {
                return m_tel;
            }
            set
            {
                m_tel = value;
            }
        }

        /// <summary>
        /// 上一次打印
        /// </summary>
        public  string Lastprint
        {
            get { return m_lastprint; }
            set { m_lastprint = value; }
        }


        /// <summary>
        /// 当前当班人
        /// </summary>
        public string CurrentOperID { get; set; }


        /// <summary>
        /// 当前当班人单号
        /// </summary>
        public string CurrentDealNo { get; set; }



        /// <summary>
        /// 当前当班人当班起始时间
        /// </summary>
        public DateTime CurrentDealTime { get; set; }




        /// <summary>
        /// 当前当班人当班钱箱初始金额
        /// </summary>
        public decimal CurrentDealMoney { get; set; }



        public  string _flow_no = string.Empty;
        /// <summary>
        /// 单号(初始赋值不变，后面赋值数字，1单号加1，2单号加2)
        /// </summary>
        public  string Flow_No
        {
            get
            {
                return _flow_no;
            }
            set
            {
                try
                {
                    if (string.IsNullOrEmpty(_flow_no))
                    {
                        _flow_no = value;
                    }
                    else
                    {
                        // _flow_no = UserData.StorageNo.Substring(0, 2) + UserData.TerminalID.ToString() + DateTime.Now.ToString("yyyyMMdd").Substring(2, 6) + Convert.ToString(Convert.ToInt32('1' + _flow_no.Substring(10, 5)) + Convert.ToInt32(value)).Substring(1, 5);
                        _flow_no = TerminalID + Convert.ToString(Convert.ToInt32(_flow_no.Substring(3, 4)) + 1).PadLeft(4, '0');
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        private  bool _SetGrant = false;
        /// <summary>
        /// 是否有前台设置权限
        /// </summary>
        public  bool SetGrant
        {
            get
            {
                return _SetGrant;
            }
            set
            {
                _SetGrant = value;
            }
        }

        private  bool _BackGoods;
        /// <summary>
        /// 是否有退货权限
        /// </summary>
        public  bool BackGoods
        {
            get
            {
                return _BackGoods;
            }
            set
            {
                _BackGoods = value;
            }
        }

        private  bool _RePrint;
        /// <summary>
        /// 是否有重打印权限
        /// </summary>
        public  bool RePrint
        {
            get
            {
                return _RePrint;
            }
            set
            {
                _RePrint = value;
            }
        }

        private  bool _openbox;
        /// <summary>
        /// 是否有开钱箱权限
        /// </summary>
        public  bool OpenBox
        {
            get
            {
                return _openbox;
            }
            set
            {
                _openbox = value;
            }
        }

        private  bool _searchSaleGrant;
        /// <summary>
        /// 是否有查询交易记录权限
        /// </summary>
        public  bool SearchSaleGrant
        {
            get
            {
                return _searchSaleGrant;
            }
            set
            {
                _searchSaleGrant = value;
            }
        }

        private  bool _SingleDaZhe = false;
        /// <summary>
        /// 是否可以单项打折
        /// </summary>
        public  bool SingleDaZhe
        {
            get
            {
                return _SingleDaZhe;
            }
            set
            {
                _SingleDaZhe = value;
            }
        }

        private  bool _AllDaZhe = false;
        /// <summary>
        /// 是否可以全部打折
        /// </summary>
        public  bool AllDaZhe
        {
            get
            {
                return _AllDaZhe;
            }
            set
            {
                _AllDaZhe = value;
            }
        }

        private  decimal _min_zk = 0;
        /// <summary>
        /// 全部打折最小折扣
        /// </summary>
        public  decimal Min_ZK
        {
            get
            {
                return _min_zk;
            }
            set
            {
                _min_zk = value;
            }
        }


        private  int _printnum = 1;
        /// <summary>
        /// 小票打印份数
        /// </summary>
        public  int PrintNum
        {
            get
            {
                return _printnum;
            }
            set
            {
                _printnum = value;
            }
        }

        //private  string _ReadCardPort = "1";
        ///// <summary>
        ///// 读卡器端口号
        ///// </summary>
        //public  string ReadCardPort
        //{
        //    get
        //    {
        //        return _ReadCardPort;
        //    }
        //    set
        //    {
        //        _ReadCardPort = value;
        //    }
        //}

        //private  ReadCardTypeEnum _readcardtype;
        ///// <summary>
        ///// 读卡器类型
        ///// </summary>
        //public  ReadCardTypeEnum ReadCardType
        //{
        //    get
        //    {
        //        return _readcardtype;
        //    }
        //    set
        //    {
        //        _readcardtype = value;
        //    }
        //}

        ///// <summary>
        ///// 读卡器类型枚举
        ///// </summary>
        //public enum ReadCardTypeEnum
        //{
        //    /// <summary>
        //    /// USB读卡器
        //    /// </summary>
        //    USB = 0,
        //    /// <summary>
        //    /// 串口读卡器
        //    /// </summary>
        //    Comm = 1
        //}

        private  string _icno = string.Empty;
        /// <summary>
        /// IC卡编号
        /// </summary>
        public  string icno
        {
            get
            {
                return _icno;
            }
            set
            {
                _icno = value;
            }
        }

        private  int _Consumeaddr = -1;
        /// <summary>
        /// 本机所在地
        /// </summary>
        public  int Consumeaddr
        {
            get
            {
                return _Consumeaddr;
            }
            set
            {
                _Consumeaddr = value;
            }
        }

        private  SaleModeEnum _SaleMode;
        /// <summary>
        /// 销售模式
        /// </summary>
        public  SaleModeEnum SaleMode
        {
            get
            {
                return _SaleMode;
            }
            set
            {
                _SaleMode = value;
            }
        }


        /// <summary>
        /// 打印类型枚举
        /// </summary>
        public enum SaleModeEnum
        {
            /// <summary>
            /// 普通小票销售模式
            /// </summary>
            Fish58 = 0,
            /// <summary>
            /// IC卡销售模式
            /// </summary>
            IC = 1
        }


        private  string _promotionname = string.Empty;
        /// <summary>
        /// 促销打包名称
        /// </summary>
        public  string PromotionName
        {
            get
            {
                return _promotionname;
            }
            set
            {
                _promotionname = value;
            }
        }

        /// <summary>
        /// 厨打服务器IP
        /// </summary>
        private  string _cookprintserver_ip = string.Empty;
        /// <summary>
        /// 厨打服务器端口
        /// </summary>
        public  string Cookprintserver_ip
        {
            get
            {
                return _cookprintserver_ip;
            }
            set
            {
                _cookprintserver_ip = value;
            }
        }

        /// <summary>
        /// 厨打服务器端口
        /// </summary>
        private  int _cookprintserver_port = 80;
        /// <summary>
        /// 厨打服务器端口
        /// </summary>
        public  int Cookprintserver_port
        {
            get
            {
                return _cookprintserver_port;
            }
            set
            {
                _cookprintserver_port = value;
            }
        }

        /// <summary>
        /// 库存参数
        /// </summary>
        public enum StorageEnum
        {
            /// <summary>
            /// 不允许
            /// </summary>
            No = 0,
            /// <summary>
            /// 允许,不提示
            /// </summary>
            Yes = 1,
            /// <summary>
            /// 允许,但提示
            /// </summary>
            YesAndAlert = 2
        }

        private  int _storageValue;
        /// <summary>
        /// 是否统计库存(0-不允许;1-允许,不提示;2-允许,但提示)
        /// </summary>
        public  int StorageValue
        {
            get
            {
                return _storageValue;
            }
            set
            {
                _storageValue = value;
            }
        }

        private  bool _isSyncServerTime = false;
        /// <summary>
        /// 是否与服务器时间同步
        /// </summary>
        public  bool SyncServerTime
        {
            get
            {
                return _isSyncServerTime;
            }
            set
            {
                _isSyncServerTime = value;
            }
        }


        private  string _posprinthead;
        /// <summary>
        /// POS销售打印头部
        /// </summary>
        public  string FishPrintHead
        {
            get
            {
                return _posprinthead;
            }
            set
            {
                _posprinthead = value;
            }
        }


        private  string _posprintbottom;
        /// <summary>
        /// POS销售打印底部
        /// </summary>
        public  string FishPrintBottom
        {
            get
            {
                return _posprintbottom;
            }
            set
            {
                _posprintbottom = value;
            }
        }

        private  string _posprinttype;
        /// <summary>
        /// POS销售打印类型
        /// </summary>
        public  string FishPrintType
        {
            get
            {
                return _posprinttype;
            }
            set
            {
                _posprinttype = value;
            }
        }

        private  string _turnclassprinthead;
        /// <summary>
        /// 交班打印头部
        /// </summary>
        public  string TurnClassPrintHead
        {
            get
            {
                return _turnclassprinthead;
            }
            set
            {
                _turnclassprinthead = value;
            }
        }

        private  string _turnclassprintbottom;
        /// <summary>
        /// 交班打印底部
        /// </summary>
        public  string TurnClassPrintBottom
        {
            get
            {
                return _turnclassprintbottom;
            }
            set
            {
                _turnclassprintbottom = value;
            }
        }

        private  string _turnclassprinttype;
        /// <summary>
        /// 交班打印底部类型
        /// </summary>
        public  string TurnClassPrintType
        {
            get
            {
                return _turnclassprinttype;
            }
            set
            {
                _turnclassprinttype = value;
            }
        }

        private  int _guqingalertqty;
        /// <summary>
        /// 估清警告提示数量
        /// </summary>
        public  int GuqingAlertQty
        {
            get
            {
                return _guqingalertqty;
            }
            set
            {
                _guqingalertqty = value;
            }
        }


        private  bool _iscookroomprint;
        /// <summary>
        /// 是否厨打
        /// </summary>
        public  bool IsCookroomPrint
        {
            get
            {
                return _iscookroomprint;
            }
            set
            {
                _iscookroomprint = value;
            }
        }


        private  int _hotTime;
        /// <summary>
        /// 热销商品刷新时间(分钟)
        /// </summary>
        public  int HotTime
        {
            get
            {
                return _hotTime;
            }
            set
            {
                _hotTime = value;
            }
        }


        private  string _pc_ip;
        /// <summary>
        /// 配餐显示屏IP
        /// </summary>
        public  string pc_ip
        {
            set { _pc_ip = value; }
            get { return _pc_ip; }
        }

        private  string _pc_line;
        /// <summary>
        /// 配餐显示在第几列
        /// </summary>
        public  string pc_line
        {
            set { _pc_line = value; }
            get { return _pc_line; }
        }

        private  bool _pc_send;
        /// <summary>
        /// 是否发送配餐信息至配餐显示屏
        /// </summary>
        public  bool pc_send
        {
            set { _pc_send = value; }
            get { return _pc_send; }
        }


        private  string _cf_ip;
        /// <summary>
        /// 厨房显示屏IP
        /// </summary>
        public  string cf_ip
        {
            set { _cf_ip = value; }
            get { return _cf_ip; }
        }

        private  bool _cf_send;
        /// <summary>
        /// 是否发送配餐信息至厨房
        /// </summary>
        public  bool cf_send
        {
            set { _cf_send = value; }
            get { return _cf_send; }
        }


        private  int _pc_port;
        /// <summary>
        /// 配餐显示屏端口
        /// </summary>
        public  int pc_port
        {
            set { _pc_port = value; }
            get { return _pc_port; }
        }


        private  int _cf_port;
        /// <summary>
        /// 厨房显示屏端口
        /// </summary>
        public  int cf_port
        {
            set { _cf_port = value; }
            get { return _cf_port; }
        }


        private  int _overtime = 3;
        /// <summary>
        /// 配餐每单超时时间（分钟）
        /// </summary>
        public  int overtime
        {
            set { _overtime = value; }
            get { return _overtime; }
        }




        /// <summary>
        /// 扫描仪类型枚举
        /// </summary>
        public enum ScannerTypeEnum
        {
            /// <summary>
            /// USB读卡器
            /// </summary>
            USB = 0,
            /// <summary>
            /// 串口读卡器
            /// </summary>
            Comm = 1
        }

        private  int _parkid = -1;
        /// <summary>
        /// 公园ID
        /// </summary>
        public  int ParkID
        {
            set { _parkid = value; }
            get { return _parkid; }
        }

        private  string _WebKeys = string.Empty;
        /// <summary>
        /// 密钥
        /// </summary>
        public  string WebKeys
        {
            set { _WebKeys = value; }
            get { return _WebKeys; }
        }

        private  int _SysitemIDForIC = -1;
        /// <summary>
        /// 一卡通系统中项目ID
        /// </summary>
        public  int SysitemIDForIC
        {
            set { _SysitemIDForIC = value; }
            get { return _SysitemIDForIC; }
        }


        private  bool _IsBuyNSendN = true;
        /// <summary>
        /// 是否参加买N送N
        /// </summary>
        public  bool IsBuyNSendN
        {
            set { _IsBuyNSendN = value; }
            get { return _IsBuyNSendN; }
        }

        private  bool _IsSendMutil = true;
        /// <summary>
        /// 是否按倍数促销
        /// </summary>
        public  bool IsSendMutil
        {
            get { return _IsSendMutil; }
            set { _IsSendMutil = value; }
        }

        private  bool _IsSendGroup = true;

        /// <summary>
        /// 是否启用组合混搭促销
        /// </summary>
        public  bool IsSendGroup
        {
            get { return _IsSendGroup; }
            set { _IsSendGroup = value; }
        }

        private  bool _IsRePrint = true;

        /// <summary>
        /// 是否启用重打印功能
        /// </summary>
        public  bool IsRePrint
        {
            get { return _IsRePrint; }
            set { _IsRePrint = value; }
        }

        private  bool _IsIdCard = true;

        /// <summary>
        /// 是否启用身份证扫描功能
        /// </summary>
        public  bool IsIdCard
        {
            get { return _IsIdCard; }
            set { _IsIdCard = value; }
        }

        private  DateTime _daysumTime = DateTime.Now;
        /// <summary>
        /// 上次日结时间
        /// </summary>
        public  DateTime DaysumTime
        {
            set { _daysumTime = value; }
            get { return _daysumTime; }
        }


        private  string _key_ok;
        /// <summary>
        /// 确定
        /// </summary>
        public  string key_ok
        {
            set { _key_ok = value; }
            get { return _key_ok; }
        }

        private  string _key_pre;
        /// <summary>
        /// 上一个
        /// </summary>
        public  string key_pre
        {
            set { _key_pre = value; }
            get { return _key_pre; }
        }

        private  string _key_next;
        /// <summary>
        /// 下一个
        /// </summary>
        public  string key_next
        {
            set { _key_next = value; }
            get { return _key_next; }
        }

        private  string _key_wait;
        /// <summary>
        /// 等待
        /// </summary>
        public  string key_wait
        {
            set { _key_wait = value; }
            get { return _key_wait; }
        }

        private  string _key_endwait;
        /// <summary>
        /// 结束等待
        /// </summary>
        public  string key_endwait
        {
            set { _key_endwait = value; }
            get { return _key_endwait; }
        }

        private  string _key_jiaohui;
        /// <summary>
        /// 叫回
        /// </summary>
        public  string key_jiaohui
        {
            set { _key_jiaohui = value; }
            get { return _key_jiaohui; }
        }

        private  string _key_make;
        /// <summary>
        /// 制作中
        /// </summary>
        public  string key_make
        {
            set { _key_make = value; }
            get { return _key_make; }
        }

        private  string _key_waitup;
        /// <summary>
        /// 等待上一个
        /// </summary>
        public  string key_waitup
        {
            set { _key_waitup = value; }
            get { return _key_waitup; }
        }

        private  string _key_waitdown;
        /// <summary>
        /// 等待下一个
        /// </summary>
        public  string key_waitdown
        {
            set { _key_waitdown = value; }
            get { return _key_waitdown; }
        }

        private  bool _tuiCai;

        public  bool TuiCai
        {
            get { return _tuiCai; }
            set { _tuiCai = value; }
        }

        private  bool _deleOrder;

        public  bool DeleOrder
        {
            get { return _deleOrder; }
            set { _deleOrder = value; }
        }

        //2014-11-7 IDC
        private  string _ConsumerKey;
        /// <summary>
        /// 
        /// </summary>
        public  string ConsumerKey
        {
            set { _ConsumerKey = value; }
            get { return _ConsumerKey; }
        }

        private  string _ConsumerSecret;
        /// <summary>
        /// 
        /// </summary>
        public  string ConsumerSecret
        {
            set { _ConsumerSecret = value; }
            get { return _ConsumerSecret; }
        }

        private  string _AccessTokenBaseUri;
        /// <summary>
        /// 
        /// </summary>
        public  string AccessTokenBaseUri
        {
            set { _AccessTokenBaseUri = value; }
            get { return _AccessTokenBaseUri; }
        }

        private  string _IDCQuery;
        /// <summary>
        /// 
        /// </summary>
        public  string IDCQuery
        {
            set { _IDCQuery = value; }
            get { return _IDCQuery; }
        }

        private  string _IDCBiz;
        /// <summary>
        /// 
        /// </summary>
        public  string IDCBiz
        {
            set { _IDCBiz = value; }
            get { return _IDCBiz; }
        }





        #region 团购订餐

        private  Dictionary<string, string> _ticketFood = new Dictionary<string, string>();

        /// <summary>
        /// 团购餐类
        /// </summary>
        public  Dictionary<string, string> TicketFood
        {
            get { return _ticketFood; }
            set { _ticketFood = value; }
        }

        private  string _ticketServerIp;

        /// <summary>
        /// 票务平台IP
        /// </summary>
        public  string TicketServerIp
        {
            get { return _ticketServerIp; }
            set { _ticketServerIp = value; }
        }

        private  string _ticketServerPort;

        /// <summary>
        /// 票务平台port
        /// </summary>
        public  string TicketServerPort
        {
            get { return _ticketServerPort; }
            set {_ticketServerPort = value; }
        }

        /// <summary>
        /// 票务平台地址
        /// </summary>
        public  string TicketServerAddr
        {
            get { return _ticketServerIp + ":" + _ticketServerPort; }

        }

        #endregion 
    }
}
