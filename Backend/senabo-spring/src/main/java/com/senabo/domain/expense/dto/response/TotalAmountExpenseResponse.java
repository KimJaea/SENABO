package com.senabo.domain.expense.dto.response;

import com.senabo.domain.expense.entity.Expense;
import lombok.Builder;

import java.time.LocalDateTime;

@Builder
public record TotalAmountExpenseResponse(
        Double totalAmount

) {
    public static TotalAmountExpenseResponse from(Double totalAmount) {
        return TotalAmountExpenseResponse.builder()
                .totalAmount(totalAmount)
                .build();
    }
}
