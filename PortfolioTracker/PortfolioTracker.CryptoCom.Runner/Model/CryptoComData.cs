using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoComTooling
{
    public class CryptoComData
    {
        [Name("Timestamp (UTC)")]
        public DateTime Timestamp{ get; set; }

        [Name("Transaction Description")]        
        public string TransactionDescription { get; set; }

        [Name("Currency")]
        public string Currency { get; set; }

        [Name("Amount")]
        public decimal Amount { get; set; }

        [Name("To Currency")]
        public string ToCurrency { get; set; }

        [Name("To Amount")]
        public decimal? ToAmount { get; set; }

        [Name("Native Currency")]
        public string NativeCurrency { get; set; }

        [Name("Native Amount")]
        public decimal NativeAmount { get; set; }

        [Name("Native Amount (in USD)")]
        public decimal NativeAmountUSD { get; set; }

        [Name("Transaction Kind")]
        public TransactionKind TransactionKind { get; set; }
    }

    public enum TransactionKind
    {
        crypto_to_exchange_transfer,
        supercharger_withdrawal,
        crypto_transfer,
        referral_card_cashback,
        referral_bonus,
        crypto_earn_interest_paid,
        viban_purchase,
        mco_stake_reward,
        supercharger_deposit,
        crypto_exchange,
        reimbursement,
        reimbursement_reverted,
        rewards_platform_deposit_credited,
        crypto_earn_program_created,
        lockup_upgrade,
        referral_gift,
        lockup_lock,
        crypto_deposit,
        crypto_viban_exchange,
        crypto_withdrawal,
        crypto_purchase,
        supercharger_reward_to_app_credited,
        card_cashback_reverted,
        crypto_earn_program_withdrawn,
        dust_conversion_credited,
        dust_conversion_debited,
        exchange_to_crypto_transfer,
        viban_deposit,
        viban_card_top_up,
        viban_withdrawal,
        crypto_viban,
        admin_wallet_credited,
        crypto_wallet_swap_credited,
        crypto_wallet_swap_debited
    }
}
