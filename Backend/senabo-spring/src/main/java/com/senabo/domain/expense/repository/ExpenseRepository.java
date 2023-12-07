package com.senabo.domain.expense.repository;

import com.senabo.domain.expense.entity.Expense;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface ExpenseRepository extends JpaRepository<Expense, String>,ExpenseRepositoryCustom {
    List<Expense> deleteByMemberId(Member member);
}
