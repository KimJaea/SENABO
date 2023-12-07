package com.senabo.domain.disease.repository;

import com.senabo.domain.disease.entity.Disease;
import com.senabo.domain.member.entity.*;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface DiseaseRepository extends JpaRepository<Disease, String>, DiseaseRepositoryCustom {
    List<Disease> deleteByMemberId(Member memberId);


}
