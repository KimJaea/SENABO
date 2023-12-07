package com.senabo.domain.expense.repository;

import com.senabo.domain.expense.entity.Expense;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;


public interface ExpenseRepositoryCustom {
    List<Expense> findByMemberId(Member member);
    List<Expense> findExpenseWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);
    Double getTotalAmount(Member member);
    Double getTotalAmountWeek(Member member, LocalDateTime endTime, LocalDateTime startTime);

    List<Expense> findTodayExpense(Member member);
}
