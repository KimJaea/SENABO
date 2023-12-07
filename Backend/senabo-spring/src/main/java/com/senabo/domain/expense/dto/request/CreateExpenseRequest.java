package com.senabo.domain.expense.dto.request;

public record CreateExpenseRequest(
        String item,
        String detail,
        Double amount
) {
}
