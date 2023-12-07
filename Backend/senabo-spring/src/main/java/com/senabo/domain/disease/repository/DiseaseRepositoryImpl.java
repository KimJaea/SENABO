package com.senabo.domain.disease.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.disease.entity.Disease;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;

import static com.senabo.domain.disease.entity.QDisease.disease;

@RequiredArgsConstructor
public class DiseaseRepositoryImpl implements DiseaseRepositoryCustom{
    private final JPAQueryFactory queryFactory;
    @Override
    public List<Disease> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(disease)
                .where(disease.memberId.eq(member))
                .orderBy(disease.createTime.desc())
                .fetch();
    }
    @Override
    public List<Disease> findLastWeekData(Member member, LocalDateTime lastStart) {
        return queryFactory
                .selectFrom(disease)
                .where(
                        disease.memberId.eq(member),
                        disease.createTime.goe(lastStart)
                )
                .orderBy(disease.createTime.desc())
                .fetch();
    }

    @Override
    public Long countLastWeekData(Member memberId, LocalDateTime lastStart) {
        return queryFactory
                .select(disease.count())
                .from(disease)
                .where(
                        disease.memberId.eq(memberId),
                        disease.createTime.goe(lastStart)
                )
                .fetchOne();
    }

    @Override
    public List<Disease> findDiseaseWeek(Member member, LocalDateTime endTime, LocalDateTime startTime) {
        return queryFactory
                .selectFrom(disease)
                .where(
                        disease.memberId.eq(member),
                        disease.updateTime.loe(endTime),
                        disease.createTime.goe(startTime)
                )
                .orderBy(disease.createTime.desc())
                .fetch();
    }
}
