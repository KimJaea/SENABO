package com.senabo.domain.report.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.report.entity.Report;
import lombok.RequiredArgsConstructor;

import java.util.List;
import java.util.Optional;

import static com.senabo.domain.report.entity.QReport.report;

@RequiredArgsConstructor
public class ReportRepositoryImpl implements ReportRepositoryCustom {
    private final JPAQueryFactory queryFactory;


    @Override
    public List<Report> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(report)
                .where(report.memberId.eq(member))
//                .orderBy(report.createTime.desc())
                .fetch();
    }

    @Override
    public Optional<Report> findLatestData(Member member) {
        return Optional.ofNullable(queryFactory
                .selectFrom(report)
                .where(report.memberId.eq(member))
                .orderBy(report.updateTime.desc())
                .limit(1)
                .fetchOne());
    }

    @Override
    public List<Report> findCompleteReport(Member member) {
        return queryFactory
                .selectFrom(report)
                .where(
                        report.memberId.eq(member),
                        report.complete.isTrue()
                )
                .orderBy(report.createTime.desc())
                .fetch();
    }

    @Override
    public Optional<Report> findCompleteReportWeek(Member member, int week) {
        return Optional.ofNullable(queryFactory
                .selectFrom(report)
                .where(
                        report.memberId.eq(member),
                        report.complete.isTrue(),
                        report.week.eq(week)
                )
                .limit(1)
                .fetchOne());
    }
}