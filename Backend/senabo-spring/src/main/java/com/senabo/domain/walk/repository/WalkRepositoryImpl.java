package com.senabo.domain.walk.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.walk.entity.Walk;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

import static com.senabo.domain.walk.entity.QWalk.walk;

@RequiredArgsConstructor
public class WalkRepositoryImpl implements WalkRepositoryCustom {
    private final JPAQueryFactory queryFactory;


    @Override
    public List<Walk> findByMemberId(Member member) {
        return queryFactory
                .selectFrom(walk)
                .where(walk.memberId.eq(member))
                .orderBy(walk.createTime.desc())
                .fetch();
    }

    @Override
    public Optional<Walk> findLatestData(Member member) {
        return Optional.ofNullable(queryFactory
                .selectFrom(walk)
                .where(walk.memberId.eq(member))
                .orderBy(walk.updateTime.desc())
                .limit(1)
                .fetchOne());
    }

    @Override
    public Optional<Walk> findTodayDataLatest(Member member, LocalDateTime todayStartTime) {
        return Optional.ofNullable(queryFactory
                .selectFrom(walk)
                .where(walk.memberId.eq(member), walk.endTime.goe(todayStartTime))
                .orderBy(walk.endTime.desc())
                .limit(1)
                .fetchOne());
    }

    @Override
    public List<Walk> findTodayData(Member member, LocalDateTime startToday) {
        return queryFactory
                .selectFrom(walk)
                .where(
                        walk.memberId.eq(member),
                        walk.startTime.goe(startToday),
                        walk.endTime.isNotNull()
                )
                .orderBy(walk.createTime.desc())
                .fetch();
    }

    @Override
    public List<Walk> findWalkWeek(Member member, LocalDateTime endTime, LocalDateTime startTime) {
        return queryFactory
                .selectFrom(walk)
                .where(
                        walk.memberId.eq(member),
                        walk.updateTime.loe(endTime),
                        walk.createTime.goe(startTime)
                )
                .orderBy(walk.createTime.desc())
                .fetch();
    }
}
