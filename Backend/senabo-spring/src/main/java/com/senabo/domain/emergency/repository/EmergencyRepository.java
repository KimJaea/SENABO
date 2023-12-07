package com.senabo.domain.emergency.repository;

import com.senabo.domain.emergency.entity.Emergency;
import com.senabo.domain.emergency.entity.EmergencyType;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository

public interface EmergencyRepository extends JpaRepository<Emergency, String>, EmergencyRepositoryCustom {
    Optional<Emergency> findByIdAndMemberId(Long id, Member member);

}
