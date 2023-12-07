package com.senabo.domain.affection.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.affection.entity.Affection;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;


import static com.senabo.domain.affection.entity.QAffection.affection;

@RequiredArgsConstructor
public class AffectionRepositoryImpl implements AffectionRepositoryCustom {
    private final JPAQueryFactory queryFactory;

    @Override
    public List<Affection> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(affection)
                .where(affection.memberId.eq(member))
                .orderBy(affection.createTime.desc())
                .fetch();
    }
    @Override
    public Optional<Affection> findLatestDataByMemberId(Member member) {
        return Optional.ofNullable(queryFactory
                .selectFrom(affection)
                .where(affection.memberId.eq(member))
                .orderBy(affection.createTime.desc())
                .fetchFirst());
    }
    @Override
    public List<Affection> findAffectionWeek(Member member, LocalDateTime startTime, LocalDateTime endTime) {
        return queryFactory
                .selectFrom(affection)
                .where(
                        affection.memberId.eq(member),
                        affection.updateTime.loe(endTime),
                        affection.createTime.goe(startTime)
                )
                .orderBy(affection.createTime.desc())
                .fetch();
    }
}
