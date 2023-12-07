package com.senabo.domain.expense.dto.response;

import com.senabo.domain.expense.entity.Expense;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record ExpenseResponse(
        Long id,
        Long memberId,
        String item,
        String detail,
        Double amount,
        LocalDateTime createTime,
        LocalDateTime updateTime
) {
    public static ExpenseResponse from(Expense expense) {
        return ExpenseResponse.builder()
                .id(expense.getId())
                .memberId(expense.getMemberId().getId())
                .item(expense.getItem())
                .detail(expense.getDetail())
                .amount(expense.getAmount())
                .createTime(expense.getCreateTime())
                .updateTime(expense.getUpdateTime())
                .build();
    }
}
