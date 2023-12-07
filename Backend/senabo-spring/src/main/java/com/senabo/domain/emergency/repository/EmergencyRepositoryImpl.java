package com.senabo.domain.emergency.repository;

import com.querydsl.jpa.impl.JPAQueryFactory;
import com.senabo.domain.emergency.entity.Emergency;
import com.senabo.domain.emergency.entity.EmergencyType;
import com.senabo.domain.member.entity.Member;
import lombok.RequiredArgsConstructor;

import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.Optional;

import static com.senabo.domain.emergency.entity.QEmergency.emergency;

@RequiredArgsConstructor
public class EmergencyRepositoryImpl implements EmergencyRepositoryCustom {
    private final JPAQueryFactory queryFactory;

    @Override
    public Optional<Emergency> findLatestEmergency(Member member) {
        LocalDateTime sevenDaysAgo = LocalDateTime.now().minusDays(7).truncatedTo(ChronoUnit.DAYS);

        return Optional.ofNullable(queryFactory
                .selectFrom(emergency)
                .where(emergency.memberId.eq(member), emergency.createTime.goe(sevenDaysAgo))
                .orderBy(emergency.createTime.desc())
                .fetchFirst());

    }

    @Override
    public List<Emergency> findLastWeekEmergency(Member member) {
        LocalDateTime sevenDaysAgo = LocalDateTime.now().minusDays(7).truncatedTo(ChronoUnit.DAYS);

        return queryFactory
                .selectFrom(emergency)
                .where(emergency.memberId.eq(member), emergency.createTime.goe(sevenDaysAgo))
                .orderBy(emergency.createTime.desc())
                .fetch();
    }

    @Override
    public List<Emergency> findByTypeToday(Member member, EmergencyType type) {
        LocalDateTime today = LocalDateTime.now().truncatedTo(ChronoUnit.DAYS);

        return queryFactory
                .selectFrom(emergency)
                .where(emergency.memberId.eq(member), emergency.createTime.goe(today), emergency.type.eq(type))
                .orderBy(emergency.createTime.desc())
                .fetch();
    }

    @Override
    public Optional<Emergency> findUnsolvedEmergency(Member member, EmergencyType type) {
        LocalDateTime sevenDaysAgo = LocalDateTime.now().minusDays(7).truncatedTo(ChronoUnit.DAYS);
        return Optional.ofNullable(queryFactory
                .selectFrom(emergency)
                .where(emergency.memberId.eq(member),
                        emergency.type.eq(type),
                        emergency.solved.isFalse(),
                        emergency.createTime.goe(sevenDaysAgo))
                .orderBy(emergency.createTime.desc())
                .limit(1)
                .fetchOne());
    }
}
