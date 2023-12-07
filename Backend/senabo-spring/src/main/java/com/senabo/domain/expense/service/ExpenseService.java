package com.senabo.domain.expense.service;

import com.senabo.domain.expense.dto.request.CreateExpenseRequest;
import com.senabo.domain.expense.dto.response.ExpenseResponse;
import com.senabo.domain.expense.dto.response.TotalAmountExpenseResponse;
import com.senabo.domain.expense.entity.Expense;
import com.senabo.domain.expense.repository.ExpenseRepository;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.exception.message.ExceptionMessage;
import com.senabo.exception.model.UserException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service
@Transactional(readOnly = true)
@RequiredArgsConstructor
public class ExpenseService {
    private final ExpenseRepository expenseRepository;
    private final MemberService memberService;

    @Transactional
    public ExpenseResponse createExpense(String email, CreateExpenseRequest request) {
        Member member = memberService.findByEmail(email);
        Expense expense = expenseRepository.save(
                new Expense(member, request.item(), request.detail(), request.amount()));
        try {
            expenseRepository.flush();
        } catch (DataIntegrityViolationException e) {
            throw new UserException(String.valueOf(ExceptionMessage.FAIL_SAVE_DATA));
        }
        return ExpenseResponse.from(expense);
    }

    @Transactional
    public List<Expense> getExpense(String email) {
        Member member = memberService.findByEmail(email);
        List<Expense> expenseList = expenseRepository.findByMemberId(member);
        return expenseList;
    }

    @Transactional
    public TotalAmountExpenseResponse getExpenseTotal(String email) {
        Member member = memberService.findByEmail(email);
        Double totalAmount = expenseRepository.getTotalAmount(member);
        log.info("======================================");
        log.info("total Amount : {}", totalAmount);
        log.info("======================================");
        if (totalAmount == null) totalAmount = 0.0;
        return TotalAmountExpenseResponse.from(totalAmount);
    }

    @Transactional
    public TotalAmountExpenseResponse getExpenseTotalWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        Double totalAmount = expenseRepository.getTotalAmountWeek(member, endTime, startTime);
        if (totalAmount == null) totalAmount = 0.0;
        return TotalAmountExpenseResponse.from(totalAmount);
    }


    @Transactional
    public List<Expense> getExpenseWeek(Report report, Member member) {
        LocalDateTime startTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS);
        LocalDateTime endTime = report.getCreateTime().truncatedTo(ChronoUnit.DAYS).plusDays(7);
        List<Expense> expenseList = expenseRepository.findExpenseWeek(member, endTime, startTime);
        return expenseList;
    }

    @Transactional
    public void removeExpense(String email) {
        try {
            Member member = memberService.findByEmail(email);
            List<Expense> list = expenseRepository.deleteByMemberId(member);
        } catch (DataIntegrityViolationException e) {
            throw new UserException(ExceptionMessage.FAIL_DELETE_DATA);
        }
    }

    public List<Expense> findTodayExpense(Member member){
        return expenseRepository.findTodayExpense(member);
    }
}
