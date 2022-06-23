using AutoMapper;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Models;
using Puzzle.Masroofi.Core.ViewModels.ATMCardTransactions;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.ParentWalletTransactions;
using Puzzle.Masroofi.Core.ViewModels.POSMachineTransactions;
using System.Linq;

namespace Puzzle.Masroofi.Core.Mapper
{
    public class TransactionsProfile : Profile
    {
        public TransactionsProfile()
        {
            CreateMap<ParentWalletTransaction, ParentWalletTransactionOutputViewModel>()
                .ForMember(dest => dest.PaymentTypeText, cfg => cfg.MapFrom(src => src.PaymentType.GetDescription()));

            CreateMap<ATMCardTransaction, ATMCardTransactionOutputViewModel>()
                .ForMember(dest => dest.PaymentTypeText, cfg => cfg.MapFrom(src => src.PaymentType.GetDescription()));

            CreateMap<POSMachineTransaction, POSMachineTransactionOutputViewModel>()
                .ForMember(dest => dest.SonNameAr, cfg => cfg.MapFrom(src => src.Son.SonNameAr))
                .ForMember(dest => dest.SonNameEn, cfg => cfg.MapFrom(src => src.Son.SonNameEn))
                .ForMember(dest => dest.PaymentTypeText, cfg => cfg.MapFrom(src => src.PaymentType.GetDescription()))
                .ForMember(dest => dest.Refunds, cfg => cfg.MapFrom(src => src.RefundTransactions.Sum(r => r.Amount)))
                .ForMember(dest => dest.NetAmount, cfg => cfg.MapFrom(src => src.Amount + src.RefundTransactions.Sum(r => r.Amount)))
                .AfterMap<MapPOSMachineTransactionSonName>();

            CreateMap<POSMachineTransaction, POSMachineRefundTransactionOutputViewModel>()
                .ForMember(dest => dest.PaymentTypeText, cfg => cfg.MapFrom(src => src.PaymentType.GetDescription()));
        }
    }
    public class MapPOSMachineTransactionSonName : IMappingAction<POSMachineTransaction, POSMachineTransactionOutputViewModel>
    {
        private readonly UserIdentity _userIdentity;
        public MapPOSMachineTransactionSonName(UserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
        }
        public void Process(POSMachineTransaction source, POSMachineTransactionOutputViewModel destination, ResolutionContext context)
        {
            if (source.Son != null)
                destination.SonName = _userIdentity.Language == Language.en ? source.Son.SonNameEn : source.Son.SonNameAr;
        }
    }
}
